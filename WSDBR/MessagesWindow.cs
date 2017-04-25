using System;
using System.Collections.Generic;
using GLib;
using Gtk;
using Mono.Data.Sqlite;
using PearXLib;
using PearXLib.GTK;
using PearXLib.GTK.Controls;
using WSDBR.Model;
using Application = Gtk.Application;
using EventArgs = System.EventArgs;
using Thread = System.Threading.Thread;

namespace WSDBR
{
    public class MessagesWindow : LoadableWindow
    {
        public Notebook Notebook = new Notebook();
        public Dictionary<string, EventyTreeView> Entries = new Dictionary<string, EventyTreeView>();

        public MessagesWindow()
        {
            Shown += OnShown;

            Title = "Messages";

            Notebook.TabPos = PositionType.Top;
            Notebook.ShowTabs = true;
            Notebook.ShowBorder = true;
            SetSizeRequest(0, 300);
            Add(Notebook);
        }

        private void OnShown(object sender, EventArgs eventArgs)
        {
            new Thread(() =>
            {
                StartLoading();
                List<Message> msgs = new List<Message>();
                string dbfile = System.IO.Path.Combine(MainWindow.Instance.DbsPath, "msgstore.db");
                using (SqliteConnection conn = new SqliteConnection($"Data Source=\"{dbfile}\"; Version=3;"))
                {
                    conn.Open();
                    using (SqliteCommand cmd = new SqliteCommand("SELECT * FROM `messages` ORDER BY `timestamp` DESC;", conn))
                    {
                        var v = cmd.ExecuteListRows();
                        foreach (var dict in v)
                        {
                            Message msg = new Message
                            {
                                Jid = (string) dict["key_remote_jid"],
                                RemoteJid = (string) dict["remote_resource"],
                                Data = (string) dict["data"],
                                FromMe = Convert.ToBoolean((long) dict["key_from_me"]),
                                Timestamp =  Program.GetWhatsAppDateTime((long)dict["timestamp"])
                            };
                            msgs.Add(msg);
                        }
                    }
                    conn.Close();
                }

                List<String> added = new List<string>();
                foreach (var msg in msgs)
                {
                    string name =  MainWindow.Instance.GetNameFromJid(msg.Jid);

                    if (!Entries.ContainsKey(name))
                    {
                        ScrolledWindow win = new ScrolledWindow();
                        EventyTreeView v = new EventyTreeView();
                        v.Prepare(GType.String, GType.String, GType.String);
                        v.AppendColumn("From");
                        v.AppendColumn("Time");
                        v.AppendColumn("Text");
                        Entries[name] = v;
                        win.Add(v);
                        Application.Invoke((s, e) =>
                        {
                            Notebook.AppendPage(win, new Label(name));
                        });
                    }
                    string str = MainWindow.Instance.GetNameFromJid(msg.RemoteJid);
                    Entries[name].AppendValues(msg.FromMe ? "You" : (str == "<none>" ? "Interlocutor" : str), msg.Timestamp.ToString(), msg.Data);
                }
                Application.Invoke((s, e) => Notebook.ShowAll());
                StopLoading();
            }).Start();
        }
    }
}
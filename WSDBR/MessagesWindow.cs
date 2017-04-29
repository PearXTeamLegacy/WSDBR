using System;
using System.Collections.Generic;
using System.Globalization;
using GLib;
using Gtk;
using Mono.Data.Sqlite;
using PearXLib;
using PearXLib.GTK;
using PearXLib.GTK.Controls;
using WSDBR.Model;
using EventArgs = System.EventArgs;

namespace WSDBR
{
    public class MessagesWindow : LoadableWindow
    {
        public Notebook Notebook = new Notebook();

        public MessagesWindow()
        {
            Shown += OnShown;

            Title = "Messages";

            Notebook.TabPos = PositionType.Top;
            Notebook.Scrollable = true;
            SetSizeRequest(0, 300);
            Add(Notebook);
        }

        private void OnShown(object sender, EventArgs eventArgs)
        {
            foreach (var cont in MainWindow.Instance.Contacts)
            {
                string name = MainWindow.Instance.GetNameFromJid(cont.JId);
                VBox vb = new VBox();
                ScrolledWindow win = new ScrolledWindow();
                EventyTreeView v = new EventyTreeView();
                Button btnRead = new Button("<");
                v.Prepare(GType.String, GType.String, GType.String);
                v.AppendColumn("From");
                v.AppendColumn("Time");
                v.AppendColumn("Text");
                btnRead.Clicked += (o, args) =>
                {
                    GetMessages(v, cont.JId);
                };
                vb.PackStart(win, true, true, 3);
                win.Add(v);
                vb.PackEnd(btnRead, false, false, 3);
                Notebook.AppendPage(vb, new Label(name));
            }
            Notebook.ShowAll();
        }

        public void GetMessages(EventyTreeView view, string jid)
        {
            string dbpath = System.IO.Path.Combine(MainWindow.Instance.DbsPath, "msgstore.db");
            using (SqliteConnection conn = new SqliteConnection($"Data Source=\"{dbpath}\"; Version=3;"))
            {
                conn.Open();
                using(SqliteCommand cmd = new SqliteCommand($"SELECT * FROM `messages` WHERE `key_remote_jid` LIKE @jid ORDER BY `timestamp` DESC;", conn))
                {
                    cmd.Parameters.AddWithValue("@jid", SqlUtils.EscapePrepared(jid));
                    cmd.Prepare();
                    try
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
                                Timestamp = Program.GetWhatsAppDateTime((long) dict["timestamp"])
                            };
                            string str = MainWindow.Instance.GetNameFromJid(msg.RemoteJid);
                            view.AppendValues(msg.FromMe ? "You" : (str == "<none>" ? "Interlocutor" : str),
                                msg.Timestamp.ToString(CultureInfo.CurrentCulture), msg.Data);
                        }
                    }
                    catch
                    {
                    }
                }
                conn.Close();
            }
        }
    }
}
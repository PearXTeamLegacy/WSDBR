using System.Collections.Generic;
using GLib;
using Gtk;
using Mono.Data.Sqlite;
using PearXLib;
using PearXLib.GTK;
using PearXLib.GTK.Controls;
using WSDBR.Model;
using EventArgs = System.EventArgs;
using Thread = System.Threading.Thread;

namespace WSDBR
{
    public class ContactsWindow : LoadableWindow
    {
        public EventyTreeView Entries = new EventyTreeView();
        public ScrolledWindow Scroll = new ScrolledWindow();

        public ContactsWindow()
        {
            Shown += OnShown;

            Title = "Contacts";

            Scroll.HscrollbarPolicy = PolicyType.Never;
            SetSizeRequest(0, 360);

            Entries.Prepare(GType.String, GType.Boolean, GType.String, GType.String, GType.String, GType.String, GType.String);
            Entries.AppendColumn("Display Name");
            Entries.AppendColumn("WhatsApp");
            Entries.AppendColumn("Status");
            Entries.AppendColumn("Status Timestamp");
            Entries.AppendColumn("Number");
            Entries.AppendColumn("WA Name");
            Entries.AppendColumn("J ID");

            Add(Scroll);
            Scroll.Add(Entries);
        }

        private void OnShown(object sender, EventArgs eventArgs)
        {
            new Thread(() =>
            {
                StartLoading();
                string dbfile = System.IO.Path.Combine(MainWindow.Instance.DbsPath, "wa.db");

                using (SqliteConnection conn = new SqliteConnection($"Data Source=\"{dbfile}\"; Version=3;"))
                {
                    conn.Open();
                    using (SqliteCommand cmd = new SqliteCommand("SELECT * FROM `wa_contacts`;", conn))
                    {
                        var v = cmd.ExecuteListRows();
                        foreach (var dict in v)
                        {
                            Contact cont = new Contact
                            {
                                DisplayName = (string) dict["display_name"],
                                JId = (string) dict["jid"],
                                Number = (string) dict["number"],
                                Status = (string) dict["status"],
                                StatusTimestamp = Program.GetWhatsAppDateTime((long)dict["status_timestamp"]),
                                WaName = (string) dict["wa_name"],
                                WhatsappUser = (bool) dict["is_whatsapp_user"]
                            };
                            Entries.AppendValues(cont.DisplayName, cont.WhatsappUser, cont.Status,
                                cont.StatusTimestamp.ToString(), cont.Number, cont.WaName, cont.JId);
                        }
                    }
                    conn.Close();
                    StopLoading();
                }
            }).Start();
        }
    }
}
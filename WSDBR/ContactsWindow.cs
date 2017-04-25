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
                foreach (var cont in MainWindow.Instance.Contacts)
                {
                    Entries.AppendValues(cont.DisplayName, cont.WhatsappUser, cont.Status,
                        cont.StatusTimestamp.ToString(), cont.Number, cont.WaName, cont.JId);
                }
                StopLoading();
            }).Start();
        }
    }
}
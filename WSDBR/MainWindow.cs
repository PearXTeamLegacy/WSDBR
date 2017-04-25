using System.Collections.Generic;
using System.IO;
using Gdk;
using Gtk;
using Mono.Data.Sqlite;
using PearXLib;
using PearXLib.GTK.Controls;
using WSDBR.Model;

namespace WSDBR
{
    public class MainWindow : LoadableWindow
    {
        public static MainWindow Instance;

        public VBox Layout = new VBox();
        public VBox LayOpen = new VBox();
        public Button BtnOpen = new Button("Open directory with DBs");
        public VBox LayActions = new VBox();
        public Button BtnContacts = new Button("Contacts");
        public Button BtnMessages = new Button("Messages");
        public string DbsPath;
        public List<Contact> Contacts = new List<Contact>();

        public MainWindow()
        {
            Main = true;
            Title = "WSDBR";
            Icon = new Pixbuf(ResourceUtils.GetFromResources("WSDBR.Icon.ico"));
            BtnMessages.Sensitive = false;
            BtnContacts.Sensitive = false;

            BtnOpen.Clicked += (sender, args) =>
            {
                using (FileChooserDialog dial = new FileChooserDialog("Open databases", this,
                    FileChooserAction.SelectFolder, "Open", ResponseType.Accept, "Cancel", ResponseType.Cancel))
                {
                    if ((ResponseType)dial.Run() == ResponseType.Accept)
                    {
                        StartLoading();
                        string s = dial.Filename;
                        bool wa = File.Exists(System.IO.Path.Combine(s, "wa.db"));
                        bool msgstore  = File.Exists(System.IO.Path.Combine(s, "msgstore.db"));
                        if(wa)
                        {
                            DbsPath = s;
                            BtnOpen.Label = $"Change directory with DBs\n[{DbsPath}]";
                            BtnContacts.Sensitive = true;

                            if (msgstore)
                                BtnMessages.Sensitive = true;
                            SetupContacts();
                        }
                        StopLoading();
                    }
                    dial.Destroy();
                }
            };

            BtnContacts.Clicked += (sender, args) =>
            {
                new ContactsWindow().Show(this);
            };
            BtnMessages.Clicked += (sender, args) =>
            {
                new MessagesWindow().Show(this);
            };

            Add(Layout);
            Layout.PackStart(LayOpen, false, false, 3);
            LayOpen.PackStart(BtnOpen, false, false, 3);
            Layout.PackStart(LayActions, true, true, 3);
            LayActions.PackStart(BtnContacts, true, true, 3);
            LayActions.PackStart(BtnMessages, true, true, 3);
        }

        public void SetupContacts()
        {
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
                        Contacts.Add(cont);
                    }
                }
                conn.Close();
            }
        }

        public string GetNameFromJid(string jid)
        {
            if (string.IsNullOrEmpty(jid))
                return "<none>";
            foreach (var cont in Contacts)
            {
                if (cont.JId == jid)
                {
                    if (string.IsNullOrEmpty(cont.DisplayName))
                        return "<none>";
                    return cont.DisplayName;
                }
            }
            return "<none>";
        }
    }
}
using System.IO;
using Gdk;
using Gtk;
using PearXLib;
using PearXLib.GTK.Controls;

namespace WSDBR
{
    public class MainWindow : PXWindow
    {
        public static MainWindow Instance;

        public VBox Layout = new VBox();
        public VBox LayOpen = new VBox();
        public Button BtnOpen = new Button("Open directory with DBs");
        public VBox LayActions = new VBox();
        public Button BtnContacts = new Button("Contacts");
        public Button BtnMessages = new Button("Messages");
        public string DbsPath;

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
                        string s = dial.Filename;
                        bool wa = File.Exists(System.IO.Path.Combine(s, "wa.db"));
                        bool msgstore  = File.Exists(System.IO.Path.Combine(s, "msgstore.db"));
                        if(wa || msgstore)
                        {
                            DbsPath = s;
                            BtnOpen.Label = $"Change directory with DBs\n[{DbsPath}]";

                        }
                        if (wa)
                            BtnContacts.Sensitive = true;
                        if (msgstore)
                            BtnMessages.Sensitive = true;
                    }
                    dial.Destroy();
                }
            };

            BtnContacts.Clicked += (sender, args) =>
            {
                new ContactsWindow().Show(this);
            };

            Add(Layout);
            Layout.PackStart(LayOpen, false, false, 3);
            LayOpen.PackStart(BtnOpen, false, false, 3);
            Layout.PackStart(LayActions, true, true, 3);
            LayActions.PackStart(BtnContacts, true, true, 3);
            LayActions.PackStart(BtnMessages, true, true, 3);

        }
    }
}
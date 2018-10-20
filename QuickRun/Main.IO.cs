using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml.Linq;

namespace QuickRun
{

    partial class Main : Window
    {
        FileSystemWatcher FileWatcher;
        bool FileWatcherFix = true;
        string DesignPath = null;

        public async void Action_Reload(string path)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        using (var fs = File.OpenRead(DesignPath))
                            break;
                    }
                    catch (IOException) { }
                }
            });
            Application.Current.Dispatcher.Invoke(() => 
            {
                Action_Load();
                Action_ShowFolder("#0");
                FolderHistory.Clear();
            });
        }

        public void Action_Load(string fileName = null)
        {
            XElement root;
            if (File.Exists(DesignPath))
                root = XElement.Load(DesignPath);
            else if (Path.IsPathRooted(fileName))
                root = XElement.Load(fileName);
            else
            {
                DesignPath = Util.GetExistingPath(fileName, ".", AppData);
                root = DesignPath == null ? XElement.Parse(Properties.Resources.design) : XElement.Load(DesignPath);
            }

            var plugins = new List<string>();
            var Map = new Dictionary<string, Item>();
            var Folder = new Dictionary<string, Panel>();

            int autoIndex = 0;
            ForItem(root);

            this.Map = Map;
            this.Folder = Folder;

            Task.Run(() => plugins.ForEach(p => PluginManager.LoadPlugin(p)));

            string GenerateKey(bool isFolder)
            {
                string key;
                do key = (isFolder ? "#" : "R") + (autoIndex++); while (Map.ContainsKey(key)||Folder.ContainsKey(key));
                return key;
            }
            
            StackPanel ForItem(XElement xparent, StackPanel spParent=null)
            {
                var sp = new StackPanel
                {
                    Tag = xparent.GetAttribute("Key", null) ?? GenerateKey(true),
                    Name = xparent.GetAttribute("Name", null) ?? "QuickRun"
                };
                Folder[sp.Tag.ToString()] = sp;
                foreach (var xe in xparent.Elements(nameof(Item)))
                {
                    var item = xe.FromXElement();

                    var btn = new Button() { Content = item.Name };
                    if (!item.Enabled) btn.Visibility = Visibility.Collapsed;
                    btn.Click += Button_Click;
                    if (Resources.Contains(item.Style))
                        btn.Style = Resources[item.Style] as Style;
                    sp.Children.Add(btn);

                    if (!string.IsNullOrEmpty(item.Plugin))
                        plugins.Add(item.Plugin);

                    if (xe.Element(nameof(Item)) != null)
                    {
                        btn.Tag = ForItem(xe, sp).Tag;
                        btn.AllowDrop = true;
                        btn.PreviewDragOver += Btn_PreviewDragOver;
                    }
                    else if(item.Type == ItemType.BackButton)
                    {
                        btn.Tag = spParent?.Tag;
                        btn.AllowDrop = true;
                        btn.PreviewDragOver += Btn_PreviewDragOver;
                    }
                    else
                    {
                        btn.Tag = xe.GetAttribute("Key", null) ?? GenerateKey(false);
                        item.Key = btn.Tag.ToString();
                        Map[btn.Tag.ToString()] = item;
                        if (item.AllowDrop) { btn.AllowDrop = true; btn.PreviewDrop += Btn_PreviewDrop; }
                    }
                }

                return sp;
            }
        }

        public void Action_LoadStyles(string fileName)
        {
            var resource = XamlReader.Parse(Properties.Resources.styles) as ResourceDictionary;
            Resources.MergedDictionaries.Add(resource);

            var path = Util.GetExistingPath(fileName, ".", AppData);
            if (path != null)
            {
                using (var fs = File.OpenRead(path))
                {
                    resource = XamlReader.Load(fs) as ResourceDictionary;
                    Resources.MergedDictionaries.Add(resource);
                }
            }
        }
    }
}

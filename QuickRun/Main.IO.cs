using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Linq;

namespace QuickRun
{

    partial class Main : Window
    {
        FileSystemWatcher FileWatcher;
        bool FileWatcherFix = true;
        string DesignPath = null;

        public async void Action_Reload()
        {
            await Task.Run(() =>
            {
                if (DesignPath == null)
                    return;
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
                Action_Load("design.xml");
                ShowFolder(RootItem);
                ItemFolderHistory.Clear();
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
            var loadedDesign = new HashSet<string>();
            if (DesignPath != null) loadedDesign.Add(DesignPath);

            var Folder = new Dictionary<Item, IEnumerable<Item>>();

            var rootItem = root.FromXElement();
            ForItem(root, rootItem);

            if(App.IsPluginSupported)
                Task.Run(() => plugins.ForEach(p => PluginManager.LoadPlugin(p)));

            ItemFolder = Folder;
            RootItem = rootItem;
            ItemFolderHistory.Clear();
            FocusedItem.Clear();

            void ForItem(XElement xparent, Item iparent)
            {
                var itemList = new List<Item>();
                foreach (var ixe in xparent.Elements(nameof(Item)))
                {
                    var item = ixe.FromXElement();

                    if (!string.IsNullOrEmpty(item.Plugin))
                        plugins.Add(item.Plugin);

                    if (!string.IsNullOrEmpty(item.DesignPath))
                    {
                        var path = Path.GetFullPath(item.DesignPath);
                        if (!loadedDesign.Contains(path) && GetPartialDesign(path) is XElement design)
                        {
                            ixe.Add(design.Elements(nameof(Item)).ToArray());
                            loadedDesign.Add(path);
                        }
                    }

                    if (ixe.Element(nameof(Item)) != null)
                        ForItem(ixe, item);
                    
                    itemList.Add(item);
                }
                Folder[iparent] = itemList;
            }

        }

        public XElement GetPartialDesign(string path)
        {
            if (!File.Exists(path)) return null;
            try
            {
                var xe = XElement.Load(path);
                return xe;
            }
            catch { return null; }
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

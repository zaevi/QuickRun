using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace QuickRun.Setting
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {

        readonly string AppData = Environment.ExpandEnvironmentVariables(@"%APPDATA%\QuickRun\");

        Dictionary<TreeViewItem, Item> ItemMap = new Dictionary<TreeViewItem, Item>();

        public Main()
            => InitializeComponent();

        private bool _Modified;
        public bool Modified
        {
            get => _Modified; set
            {
                _Modified = value;
                Title = (_Modified ? "*" : "") + "QuickRun配置 - " + (_FilePath ?? "New");
            }
        }

        private string _FilePath;
        public string FilePath
        {
            get => _FilePath; set
            {
                _FilePath = value;
                menuSave.IsEnabled = !(_FilePath is null);
            }
        }

        public void Action_Close()
        {
            treeView.Items.Clear();
            ItemMap.Clear();
        }

        public void Action_NewItem(ItemsControl parent=null, int index=-1)
        {
            parent = parent ?? treeView;
            if (index == -1) index = parent.Items.Count;

            var item = new Item();
            var node = new TreeViewItem() { Header = item.Name };

            ItemMap[node] = item;
            parent.Items.Insert(index, node);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if((sender as Button)?.Tag.ToString() == "Add")
            {
                if (treeView.SelectedItem is TreeViewItem selected)
                    Action_NewItem(selected.Parent as ItemsControl, selected.IndexOfParent());
                else
                    Action_NewItem();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.IO.Directory.CreateDirectory(AppData);
        }
    }
}

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

        string StyleTemplate;
        string DesignTemplate;

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

        public bool Action_Close()
        {
            if (FilePath != null && Modified)
            {
                var result = MessageBox.Show("是否保存当前文件?", "", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes) Action_Save(FilePath);
                else if (result == MessageBoxResult.Cancel) return false;
            }
            treeView.Items.Clear();
            return true;
        }

        public void Action_NewItem(ItemsControl parent=null, int index=-1)
        {
            parent = parent ?? treeView;
            if (index == -1) index = parent.Items.Count;
            var node = new TreeViewItem() { DataContext = new Item() };
            node.SetBinding(TreeViewItem.HeaderProperty, "Name");
            parent.Items.Insert(index, node);
        }

        public void Action_RemoveItem(TreeViewItem node)
            => node.RemoveFromParent();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as Button).Tag is string tag)) return;
            if(tag == "Add")
            {
                if (treeView.SelectedItem is TreeViewItem selected)
                    Action_NewItem(selected.Parent as ItemsControl, selected.IndexOfParent() + 1);
                else
                    Action_NewItem();
            }
            else if(tag == "Del")
            {
                if (!(treeView.SelectedItem is TreeViewItem selected)) return;
                if (selected.HasItems)
                {
                    MessageBox.Show("请先移除其子项!");
                    return;
                }
                Action_RemoveItem(selected);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.IO.Directory.CreateDirectory(AppData);
            try
            {
                StyleTemplate = QuickRun.Properties.Resources.styles;
                DesignTemplate = QuickRun.Properties.Resources.design;
            }
            catch(System.IO.FileNotFoundException)
            {
                var rm = new System.Resources.ResourceManager("QuickRun.Properties.Resources", typeof(Item).Assembly);
                StyleTemplate = rm.GetString("styles");
                DesignTemplate = rm.GetString("design");
            }
        }
    }
}

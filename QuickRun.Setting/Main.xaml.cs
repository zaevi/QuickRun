using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuickRun.Setting
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        bool Modified;
        string FilePath = null;

#if DEBUG
        readonly string AppData = Environment.ExpandEnvironmentVariables(@".\");
#else
        readonly string AppData = Environment.ExpandEnvironmentVariables(@"%APPDATA%\QuickRun\");
#endif

        Dictionary<TreeViewItem, Item> ItemMap = new Dictionary<TreeViewItem, Item>();

        public Main()
            => InitializeComponent();


        public void Action_New()
        {
            Action_Close();

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
    }
}

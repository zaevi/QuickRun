using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace QuickRun.Setting
{
    /// <summary>
    /// 文件读写&构建
    /// </summary>
    public partial class Main : Window
    {
        public void Action_ExportStyleTemplate(string path)
            => File.WriteAllText(path, StyleTemplate);

        public void Action_Load(string path=null)
        {
            if (!Action_Close()) return;
            var root = path != null ? XElement.Load(path) : XElement.Parse(DesignTemplate);
            
            void ForItem(XElement xparent, ItemsControl parent)
            {
                foreach(var xe in xparent.Elements(nameof(Item)))
                {
                    var item = xe.FromXElement();
                    var treeItem = new TreeViewItem() { DataContext = item };
                    treeItem.SetBinding(TreeViewItem.HeaderProperty, "Name");
                    parent.Items.Add(treeItem);
                    if (xe.Element(nameof(Item))!=null)
                        ForItem(xe, treeItem);
                }
            }

            ForItem(root, treeView);

            FilePath = path;
            Modified = false;
        }

        public void Action_Save(string path)
        {
            XElement ForItem(ItemsControl treeItem)
            {
                XElement xe;
                if (treeItem is TreeViewItem)
                {
                    xe = (treeItem.DataContext as Item).ToXElement();
                }
                else
                    xe = new XElement("Item");
                if (treeItem.HasItems)
                    foreach(TreeViewItem ti in treeItem.Items)
                        xe.Add(ForItem(ti));
                return xe;
            }

            ForItem(treeView).Save(path);

            FilePath = path;
            Modified = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml.Linq;
using System.IO;

namespace QuickRun.Setting
{
    /// <summary>
    /// 文件读写&构建
    /// </summary>
    public partial class Main : Window
    {
        public void Action_ExportStyleTemplate(string path)
            => File.WriteAllText(path, Properties.Resources.styles);

        public void Action_Load(string path=null)
        {
            Action_Close();
            var root = path != null ? XElement.Load(path) : XElement.Parse(Properties.Resources.design);
            
            void ForItem(XElement xparent, ItemsControl parent)
            {
                foreach(var xe in xparent.Elements(nameof(Item)))
                {
                    var item = xe.FromXElement();
                    var treeItem = new TreeViewItem() { Header = item.Name };
                    ItemMap[treeItem] = item;
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
                    xe = ItemMap[treeItem as TreeViewItem].ToXElement();
                }
                else
                    xe = new XElement("QuickRunSetting");
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

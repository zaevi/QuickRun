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
        public void Action_SaveTemplate(string designPath, string stylePath)
        {
            File.WriteAllText(designPath, Properties.Resources.design);
            File.WriteAllText(stylePath, Properties.Resources.styles);
        }

        public void Action_Load(string path)
        {
            Action_Close();
            var root = XElement.Load(path);
            
            void ForItem(XElement xparent, ItemsControl parent)
            {
                foreach(var xe in xparent.Elements("Item"))
                {
                    var item = xe.FromXElement();
                    var treeItem = new TreeViewItem() { Header = item.Name };
                    ItemMap[treeItem] = item;
                    parent.Items.Add(treeItem);
                    if (xe.Element("Item")!=null)
                        ForItem(xe, treeItem);
                }
            }

            ForItem(root, treeView);
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
        }

        public void Action_Build(string path)
        {
            var xroot = new XElement("Map");
            var spRoot = new StackPanel();

            var keys = new HashSet<string>();
            var autoIndex = 0;

            XElement BuildAction(Item item)
            {
                var action = new XElement("Action",
                    new XAttribute("Key", item.Key), 
                    new XAttribute("Type", Enum.GetName(typeof(ItemType), item.Type)),
                    new XAttribute("Uri", item.Uri));
                return action;
            }

            StackPanel ForItem(ItemsControl treeItem, StackPanel spParent)
            {
                StackPanel sp = new StackPanel() { Tag = "#" + autoIndex++ };
                spRoot.Children.Add(sp);
                foreach (TreeViewItem tvi in treeItem.Items)
                {
                    var item = ItemMap[tvi];
                    var btn = new Button() { Content = item.Name };
                    if(tvi.HasItems)
                    {
                        btn.Tag = ForItem(tvi, sp).Tag;
                    }
                    else if (item.Type == ItemType.BackButton)
                    {
                        btn.Tag = spParent?.Tag ?? "#0";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.Key))
                            item.Key = "A" + autoIndex++;
                        btn.Tag = item.Key;
                        xroot.Add(BuildAction(item));
                    }
                    item.Key = btn.Tag.ToString();
                    if(item.Enabled) sp.Children.Add(btn);
                }
                return sp;
            }

            ForItem(treeView, null);

            var basePath = System.IO.Path.ChangeExtension(path, null);
            xroot.Save(basePath + ".map.xml");
            using (var f = System.Xml.XmlWriter.Create(basePath + ".xaml", new System.Xml.XmlWriterSettings { Indent=true}))
            {
                XamlWriter.Save(spRoot, f);
            }
        }
    }
}

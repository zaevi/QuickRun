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
using System.IO;
using System.Windows.Markup;
using System.Collections;
using System.Diagnostics;
using System.Xml.Linq;

namespace QuickRun
{
    partial class Main : Window
    {
        public void Action_Load(string path = null)
        {
            path = Util.GetExistingPath(path, ".", AppData);

            int autoIndex = 0;

            var root = path != null ? XElement.Load(path) : XElement.Parse(Properties.Resources.design);
            ForItem(root);

            string GenerateKey(bool isFolder)
            {
                string key;
                do key = (isFolder ? "#" : "R") + (autoIndex++); while (Map.ContainsKey(key)||Folder.ContainsKey(key));
                return key;
            }
            
            StackPanel ForItem(XElement xparent, StackPanel spParent=null)
            {
                var sp = new StackPanel();
                sp.Tag = xparent.GetAttribute("Key", null) ?? GenerateKey(true);
                Folder[sp.Tag.ToString()] = sp;
                foreach (var xe in xparent.Elements(nameof(Item)))
                {
                    var item = xe.FromXElement();

                    var btn = new Button() { Content = item.Name };
                    if (!item.Enabled) btn.Visibility = Visibility.Collapsed;
                    btn.Click += Button_Click;
                    sp.Children.Add(btn);
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
                        if (item.AllowDrop) { btn.AllowDrop = true; btn.Click += Button_Click; }
                    }
                }

                return sp;
            }
        }
    }
}

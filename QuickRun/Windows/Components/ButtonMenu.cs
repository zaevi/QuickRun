using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using QuickRun.SDK;
using System.Reflection;

namespace QuickRun.Windows.Components
{
    public class ButtonMenu
    {
        private static ContextMenu s_instance;

        public static ContextMenu Instance
        {
            get
            {
                s_instance = s_instance ?? CreateButtonMenu();
                return s_instance;
            }
        }

        private static ContextMenu CreateButtonMenu()
        {
            var rootMenu = new ContextMenu();

            rootMenu.Items.Add(CreateButtonMenuItem(typeof(BaseButton)));
            rootMenu.Items.Add(CreateButtonMenuItem(typeof(Folder)));
            rootMenu.Items.Add(new Separator());

            var menu = new MenuItem() { Header = "Button..." };
            ButtonFactory.LocalButtonTypes.Where(t => !t.IsAbstract && typeof(IButton).IsAssignableFrom(t) && !typeof(IFolder).IsAssignableFrom(t))
                .ToList().ForEach(t => menu.Items.Add(CreateButtonMenuItem(t)));
            rootMenu.Items.Add(menu);

            menu = new MenuItem { Header = "Folder..." };
            ButtonFactory.LocalButtonTypes.Where(t => !t.IsAbstract && typeof(IFolder).IsAssignableFrom(t))
                .ToList().ForEach(t => menu.Items.Add(CreateButtonMenuItem(t)));
            rootMenu.Items.Add(menu);

            return rootMenu;
        }

        private static MenuItem CreateButtonMenuItem(Type type)
        {
            var attr = type.GetCustomAttribute<ConfigAttribute>();
            var name = string.IsNullOrEmpty(attr.Name) ? type.Name : $"{attr.Name}({type.Name})";

            var item = new MenuItem { Header = name, Tag = type };
            item.Click += MenuItem_Click;

            return item;
        }

        private static void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var type = (sender as MenuItem).Tag as Type;
            var config = new Config
            {
                ConfigType = type,
                Title = type.Name
            };
            config.ShowDialog();
        }
    }
}

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
using Forms = System.Windows.Forms;

namespace QuickRun
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
#if DEBUG
        readonly string AppData = Environment.ExpandEnvironmentVariables(@".\");
#else
        readonly string AppData = Environment.ExpandEnvironmentVariables(@"%APPDATA%\QuickRun\");
#endif
        public Dictionary<string, string> Map = new Dictionary<string, string>();
        public Dictionary<string, Panel> Folder = new Dictionary<string, Panel>();

        string CurrentFolder = null;

        Forms.NotifyIcon Notify;

        public Main()
            => InitializeComponent();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (sender as Button);
            if(btn.Tag is string tag)
            {
                if(tag.StartsWith("#"))
                {
                    Action_ShowFolder(tag);
                    title.Content = (sender as Button).Content;
                }
                else if(tag.StartsWith("A"))
                {
                    Action_RunAction(tag);
                }
            }
        }

        private void Btn_PreviewDragOver(object sender, DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Btn_PreviewDrop(object sender, DragEventArgs e)
        {
            if(e.Data.GetFormats().Contains("FileName"))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var btn = (sender as Button);
                if (btn.Tag is string tag && tag.StartsWith("A"))
                {
                    Action_RunAction(tag, string.Join(" ", files.Select(f => "\"" + f + "\"")));
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Action_LoadStyles(AppData + "styles.xaml");
            Action_LoadItems(AppData + "design.xaml", AppData + "design.map.xml");
            ShowInTaskbar = false;

            Notify = new Forms.NotifyIcon() {
                ContextMenu = new Forms.ContextMenu(),
                Icon = Properties.Resources.icon_notify,
                Visible = true,
            };
            Notify.MouseClick += (s, me) => { if(me.Button == Forms.MouseButtons.Left) if (IsVisible) Hide(); else Show(); };
            Notify.ContextMenu.MenuItems.Add("退出").Click += (s, ce) => Close();


            var area = SystemParameters.WorkArea;
            Top = area.Bottom - Height - 20;
            Left = area.Right - Width - 20;
        }

        private void TitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            titleBar.MouseDown += (s, me) => DragMove();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
            => Action_ShowFolder(null, true);

        private void hideBtn_Click(object sender, RoutedEventArgs e)
            => Hide();

        private void Window_Closed(object sender, EventArgs e)
        {
            Notify.Visible = false;
            Notify?.Dispose();
        }
    }
}

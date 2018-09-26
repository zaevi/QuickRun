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

        readonly string AppData = Environment.ExpandEnvironmentVariables(@"%APPDATA%\QuickRun\");

        public Dictionary<string, Item> Map = new Dictionary<string, Item>();
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
                }
                else if(tag.StartsWith("$"))
                {
                    
                }
                else
                {
                    Action_RunAction(tag);
                }
            }
        }


        private Button DragOverTarget = null;
        private int DragOverStartTime;

        private void Btn_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (!(sender is Button btn)) return;
            if(DragOverTarget != sender)
            {
                DragOverTarget = btn;
                DragOverStartTime = Environment.TickCount;
            }
            if (Environment.TickCount - DragOverStartTime > 500)
            {
                DragOverTarget.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void Btn_PreviewDrop(object sender, DragEventArgs e)
        {
            if(e.Data.GetFormats().Contains("FileName"))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var btn = (sender as Button);
                if (btn.Tag is string tag)
                {
                    Action_RunAction(tag, string.Join(" ", files.Select(f => "\"" + f + "\"")));
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(AppData);
            Action_LoadStyles("styles.xaml");
            Action_Load("design.xml");
            Action_ShowFolder("#0");

            if (DesignPath != null)
            {
                FileWatcher = new FileSystemWatcher()
                {
                    Path = System.IO.Path.GetDirectoryName(DesignPath),
                    Filter = System.IO.Path.GetFileName(DesignPath),
                    EnableRaisingEvents = true,
                };
                FileWatcher.Changed += (s, fe) =>
                {
                    if (FileWatcherFix) { FileWatcherFix = false; return; }
                    FileWatcherFix = true;
                    Action_Reload(fe.FullPath);
                };
            }

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

            backBtn.AllowDrop = true;
            backBtn.PreviewDragOver += Btn_PreviewDragOver;
            Topmost = true;
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

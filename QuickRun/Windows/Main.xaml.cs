using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using QuickRun.SDK;
using Forms = System.Windows.Forms;

namespace QuickRun.Windows
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {

        readonly string AppData = Environment.ExpandEnvironmentVariables(@"%APPDATA%\QuickRun\");

        public Dictionary<Item, IEnumerable<Item>> ItemFolder = new Dictionary<Item, IEnumerable<Item>>();
        public Item RootItem = null;
        public Item CurrentItem = null;

        public static Main Instance = null;

        Stack<Item> ItemFolderHistory = new Stack<Item>();
        Dictionary<Item, Item> FocusedItem = new Dictionary<Item, Item>();

        Forms.NotifyIcon Notify;

        public Main()
        {
            InitializeComponent();
            Instance = this;
        }

        private Button DragOverTarget = null;
        private int DragOverStartTime;

        private void Btn_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (!(sender is Button btn)) return;
            if (DragOverTarget != sender)
            {
                DragOverTarget = btn;
                DragOverStartTime = Environment.TickCount;
            }
            if (Environment.TickCount - DragOverStartTime > 500)
            {
                DragOverTarget.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Environment.GetCommandLineArgs().Contains("-h")) Hide();

            Directory.CreateDirectory(AppData);
            //Action_LoadStyles("styles.xaml");
            //Action_Load("design.xml");
            //ShowFolder(RootItem);

            //if (DesignPath != null)
            //{
            //    FileWatcher = new FileSystemWatcher()
            //    {
            //        Path = Path.GetDirectoryName(DesignPath),
            //        Filter = Path.GetFileName(DesignPath),
            //        EnableRaisingEvents = true,
            //    };
            //    FileWatcher.Changed += (s, fe) =>
            //    {
            //        if (FileWatcherFix) { FileWatcherFix = false; return; }
            //        FileWatcherFix = true;
            //        Action_Reload();
            //    };
            //}

            Notify = new Forms.NotifyIcon()
            {
                ContextMenu = new Forms.ContextMenu(),
                Icon = Properties.Resources.icon_notify,
                Visible = true,
            };
            Notify.MouseClick += (s, me) => { if (me.Button == Forms.MouseButtons.Left) if (IsVisible) Hide(); else Show(); };
            Notify.ContextMenu.MenuItems.Add("退出").Click += (s, ce) => Close();


            var area = SystemParameters.WorkArea;
            Top = area.Bottom - Height - 20;
            Left = area.Right - Width - 20;

            backBtn.AllowDrop = true;
            backBtn.PreviewDragOver += Btn_PreviewDragOver;

            PreviewMouseRightButtonUp += (s, me) => ShowFolder(null, true);
            PreviewKeyDown += Main_PreviewKeyDown;
        }

        private void TitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            titleBar.MouseLeftButtonDown += (s, me) => DragMove();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
            => ShowFolder(null, true);

        private void hideBtn_Click(object sender, RoutedEventArgs e)
            => Hide();

        private void Window_Closed(object sender, EventArgs e)
        {
            Notify.Visible = false;
            Notify?.Dispose();
        }

        private void Main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.FocusedElement == this && e.Key.HasAny(Key.Up, Key.Down, Key.Enter))
            {
                FocusItem(0);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Hide();
            }
            else if (e.Key.HasAny(Key.Left, Key.Back))
            {
                ShowFolder(null, true);
            }
            else if (e.Key == Key.Right && Keyboard.FocusedElement is Button btn && btn.DataContext is Item item)
            {
                if (ItemFolder.ContainsKey(item))
                {
                    ShowFolder(item);
                }
                else if (item.Type == ItemType.BackButton)
                {
                    ShowFolder(null, true);
                    e.Handled = true;
                }
            }
        }

        private void Button_Initialized(object sender, EventArgs e)
        {

            var btn = sender as Button;
            var button = btn.DataContext as IButton;
            button.OnInitialized(sender, e);
            btn.Style = Resources["Default"] as Style;
        }

        private void Button_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
            => FocusedItem[CurrentItem] = (sender as Button)?.DataContext as Item;

        public void FocusItem(int index)
        {
            var container = itemsControl.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
            container?.FindVisualChildren<Button>().FirstOrDefault()?.Focus();
        }

        public void FocusItem(Item item)
        {
            var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
            container?.FindVisualChildren<Button>().FirstOrDefault()?.Focus();
        }

        public void ShowFolder(object item, bool back = false)
        {

        }
    }
}

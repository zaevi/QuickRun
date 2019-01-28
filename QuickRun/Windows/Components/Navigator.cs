using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuickRun.SDK;

namespace QuickRun.Windows.Components
{
    public class Navigator
    {
        private Stack<IFolder> FolderHistory = new Stack<IFolder>();

        public virtual Main Main { get => _main; set { _main = value; Main.PreviewKeyDown += Main_PreviewKeyDown; } }

        public IFolder RootFolder { get; set; }

        public IFolder CurrentFolder { get; set; }

        public void ShowFolder(IFolder folder)
        {
            if (CurrentFolder != null)
                FolderHistory.Push(CurrentFolder);
            SetFolder(folder);
        }

        public void SetFolder(IFolder folder)
        {
            Main.itemsControl.ItemsSource = folder.Buttons;
            Main.title.Content = folder.Title ?? "";
            CurrentFolder = folder;
        }

        public void Back()
        {
            if (FolderHistory.Count == 0) return;
            var folder = FolderHistory.Pop();
            SetFolder(folder);
        }

        private Button DragOverTarget = null;
        private int DragOverStartTime;
        private Main _main;

        public void Btn_PreviewDragOver(object sender, DragEventArgs e)
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

        private void Main_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.FocusedElement == this && e.Key.HasAny(Key.Up, Key.Down, Key.Enter))
            {
                FocusItem(0);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Main.Hide();
            }
            else if (e.Key.HasAny(Key.Left, Key.Back))
            {
                Back();
            }
            else if (e.Key == Key.Right && Keyboard.FocusedElement is Button btn && btn.DataContext is IFolder folder)
            {
                ShowFolder(folder);
            }
        }

        public void FocusItem(int index)
        {
            var container = Main.itemsControl.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
            container?.FindVisualChildren<Button>().FirstOrDefault()?.Focus();
        }

        public void FocusItem(IButton button)
        {
            var container = Main.itemsControl.ItemContainerGenerator.ContainerFromItem(button) as FrameworkElement;
            container?.FindVisualChildren<Button>().FirstOrDefault()?.Focus();
        }
    }
}

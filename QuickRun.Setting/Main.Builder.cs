using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QuickRun.Setting
{
    /// <summary>
    /// 杂项
    /// </summary>
    public partial class Main : Window
    {

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as MenuItem).Tag;
            switch(tag)
            {
                case "New":
                    Action_New();
                    break;
                case "Open":
                    if(!System.IO.File.Exists(AppData+"design.xml"))
                        Action_SaveTemplate(AppData + "design.xml", AppData + "styles.xml");
                    Action_Load(AppData + "design.xml");
                    break;
                case "Save":
                    Action_Save(AppData + "design.xml");
                    break;
                case "SaveAs":
                    break;
                case "Build":
                    Action_Build(AppData + "design.xml");
                    break;
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = e.OldValue as TreeViewItem;
            if (item != null)
            {
                Action_SaveItem(ItemMap[item]);
                item.Header = ItemMap[item].Name;
            }
            item = e.NewValue as TreeViewItem;
            if (item != null)
                Action_LoadItem(ItemMap[item]);
            
        }
    }

    /// <summary>
    /// 属性编辑器生成
    /// </summary>
    public partial class Main : Window
    {
        Dictionary<PropertyInfo, PropertyValue> PropertyMap = new Dictionary<PropertyInfo, PropertyValue>();

        private void Action_SaveItem(Item item)
        {
            foreach(var pair in PropertyMap)
            {
                pair.Key.SetValue(item, pair.Value.GetValue());
            }
        }

        private void Action_LoadItem(Item item)
        {
            foreach (var pair in PropertyMap)
            {
                pair.Value.SetValue(pair.Key.GetValue(item));
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var properties = typeof(Item).GetProperties();
            for (int i = 1; i < properties.Length; i++)
                propertyGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            for (int i=0; i<properties.Length; i++)
            {
                var property = properties[i];
                var name = (property.GetCustomAttributes(false).FirstOrDefault() as NameAttribute)?.Name;
                
                var label = new Label() { Content = name };
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);
                propertyGrid.Children.Add(label);

                var pv = new PropertyValue(property.PropertyType);
                pv.Element.VerticalAlignment = VerticalAlignment.Center;
                pv.Element.Margin = new Thickness(5);
                Grid.SetRow(pv.Element, i);
                Grid.SetColumn(pv.Element, 1);
                propertyGrid.Children.Add(pv.Element);

                PropertyMap[property] = pv;
            }
        }
    }

    internal class PropertyValue
    {
        public Func<object> GetValue;
        public Action<object> SetValue;

        public FrameworkElement Element;
        
        public PropertyValue(Type type)
        {
            if(type == typeof(string))
            {
                Element = new TextBox();
                GetValue = () => (Element as TextBox).Text;
                SetValue = (t) => (Element as TextBox).Text = t?.ToString();
            }
            else if(type == typeof(bool))
            {
                Element = new CheckBox();
                GetValue = () => (Element as CheckBox).IsChecked;
                SetValue = (c) => (Element as CheckBox).IsChecked = (bool)c;
            }
            else if(type.IsEnum)
            {
                Element = new ComboBox();
                foreach(var name in Enum.GetNames(type))
                {
                    (Element as ComboBox).Items.Add(new ComboBoxItem() { Content = name });
                }
                GetValue = () => (Element as ComboBox).SelectedIndex;
                SetValue = (t) => (Element as ComboBox).SelectedIndex = (int)t;
            }
        }
    }

    /// <summary>
    /// 树形列表交互部分
    /// </summary>
    public partial class Main : Window
    {

        public void MoveItem(TreeViewItem source, ItemsControl targetParent, int targetIndex)
        {
            source.RemoveFromParent();
            targetParent.Items.Insert(Math.Min(targetIndex, targetParent.Items.Count), source);
        }

        public void MoveTo(string d)
        {
            var node = treeView.SelectedItem as TreeViewItem;
            if (node is null) return;

            var parent = node.Parent as ItemsControl;
            var nodeIndex = parent.Items.IndexOf(node);

            switch (d)
            {
                case "U":
                    if (nodeIndex == 0) break;
                    MoveItem(node, parent, nodeIndex - 1);
                    break;
                case "D":
                    if (parent.Items.Count - 1 == nodeIndex) break;
                    MoveItem(node, parent, nodeIndex + 1);
                    break;
                case "L":
                    if (parent == treeView) break;
                    var targetParent = parent.Parent as ItemsControl;
                    MoveItem(node, targetParent, targetParent.Items.IndexOf(parent) + 1);
                    break;
                case "R":
                    if (nodeIndex == 0) break;
                    targetParent = parent.Items[nodeIndex - 1] as ItemsControl;
                    MoveItem(node, targetParent, targetParent.Items.Count);
                    (targetParent as TreeViewItem).IsExpanded = true;
                    break;
            }
            node.IsSelected = true;
        }

        //
        // Key & Button Method
        //
        ICommand IMove = new RoutedUICommand();

        private void treeView_Loaded(object sender, RoutedEventArgs _e)
        {
            treeView.PreviewMouseMove += treeView_PreviewMouseMove;
            treeView.DragOver += treeView_DragOver;
            treeView.Drop += treeView_Drop;
            treeView.CommandBindings.Add(new CommandBinding(IMove, (s, e) => MoveTo(e.Parameter.ToString())));
            treeView.InputBindings.AddRange(new[] {
                new KeyBinding(IMove, Key.Up, ModifierKeys.Control) {CommandParameter="U"},
                new KeyBinding(IMove, Key.Down, ModifierKeys.Control){CommandParameter="D"},
                new KeyBinding(IMove, Key.Left, ModifierKeys.Control){CommandParameter="L"},
                new KeyBinding(IMove, Key.Right, ModifierKeys.Control){CommandParameter="R"},
            });
        }

        private void MoveButton_Click(object s, RoutedEventArgs e)
            => MoveTo((s as Button).Tag.ToString());

        //
        // DragDrop Method
        //
        TreeViewItem SourceItem = null;
        TreeViewItem DragOverItem = null;
        ItemsControl TargetItem = null;
        int TargetIndex = 0;

        private void treeView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            var result = VisualTreeHelper.HitTest(treeView, e.GetPosition(treeView));

            if (result == null || !(result.VisualHit is TextBlock tb)) return;
            if (!(tb.GetVisualParent<TreeViewItem>() is TreeViewItem item)) return;

            bool expanded = item.HasItems && (item as TreeViewItem).IsExpanded;
            if (expanded) (item as TreeViewItem).IsExpanded = false;
            SourceItem = item;
            DragDrop.DoDragDrop(treeView, item, DragDropEffects.Move);
            if (expanded) (item as TreeViewItem).IsExpanded = true;
            if (DragOverItem != null)
            {
                DragOverItem.BorderThickness = new Thickness();
                DragOverItem.IsSelected = false;
                DragOverItem = null;
            }
            TargetItem = null;
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            var result = VisualTreeHelper.HitTest(treeView, e.GetPosition(treeView));
            if (result.VisualHit is TextBlock tb)
            {
                if (DragOverItem != null)
                {
                    DragOverItem.BorderThickness = new Thickness();
                    DragOverItem.IsSelected = false;
                }
                var parent = result.VisualHit.GetVisualParent<TreeViewItem>();
                var pos = (parent.Parent as ItemsControl).Items.IndexOf(parent);
                var h = e.GetPosition(parent).Y / (result.VisualHit as TextBlock).RenderSize.Height;
                if (parent == SourceItem) return;
                if (h < 0.25)
                {
                    parent.BorderThickness = new Thickness(0, 1, 0, 0);
                    TargetItem = parent.Parent as ItemsControl;
                    TargetIndex = pos;
                }
                else if (h > 0.75)
                {
                    parent.BorderThickness = new Thickness(0, 0, 0, 1);
                    TargetItem = parent.Parent as ItemsControl;
                    TargetIndex = pos + 1;
                }
                else
                {
                    parent.IsSelected = true;
                    TargetItem = parent;
                    TargetIndex = TargetItem.Items.Count;
                }
                DragOverItem = parent;
            }
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            if (TargetItem == null) return;
            MoveItem(SourceItem, TargetItem, TargetIndex);
        }
    }


}

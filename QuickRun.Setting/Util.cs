using System;
using System.Windows;
using System.Windows.Controls;

namespace QuickRun.Setting
{
    public static class Util
    {
        public static T GetVisualParent<T>(this DependencyObject dObject) where T: Control
        {
            var obj = dObject as FrameworkElement;
            while (obj != null)
            {
                if (obj is T)
                    return (T)obj;
                obj = obj.TemplatedParent as FrameworkElement;
            }
            return null;
        }

        public static void RemoveFromParent(this FrameworkElement child)
            => (child.Parent as ItemsControl)?.Items.Remove(child);

        public static int IndexOfParent(this ItemsControl child)
        {
            var parent = child.Parent as ItemsControl;
            return (parent == null) ? -1 :parent.Items.IndexOf(child);
        }

        public static object Parse(this Type type, string value)
        {
            if (type.IsEnum)
                return Enum.Parse(type, value);
            return Convert.ChangeType(value, type);
        }
    }
}

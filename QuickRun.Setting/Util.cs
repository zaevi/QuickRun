using System;
using System.IO;
using System.Linq;
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
            => (!(child.Parent is ItemsControl parent)) ? -1 : parent.Items.IndexOf(child);

        public static string GetExistingPath(string fileName, params string[] folderPath)
            => folderPath.Select(f => Path.Combine(f, fileName)).Where(f => File.Exists(f)).FirstOrDefault();
    }
}

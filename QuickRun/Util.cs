using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace QuickRun
{
    public static class Util
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        public static string GetAttribute(this XElement element, XName name, string defaultValue="")
        {
            if (element.Attribute(name) is XAttribute attr)
                return attr.Value;
            return defaultValue;
        }

        public static string GetExistingPath(string fileName, params string[] folderPath)
            => folderPath.Select(f => Path.Combine(f, fileName)).Where(f => File.Exists(f)).FirstOrDefault();

        public static object Parse(this Type type, string value)
        {
            if (type.IsEnum)
                return Enum.Parse(type, value);
            return Convert.ChangeType(value, type);
        }
    }
}

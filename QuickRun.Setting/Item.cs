using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace QuickRun.Setting
{
    public enum ItemType
    {
        Uri,
        Process,
        BackButton,
    }

    public class Item
    {
        [Name("名称")]
        public string Name { get; set; } = "Node";

        [Name("类型")]
        public ItemType Type { get; set; } = ItemType.Uri;

        [Name("Key")]
        public string Key { get; set; } = "";

        [Name("启动Uri")]
        public string Uri { get; set; } = "";

        [Name("允许拖拽")]
        public bool AllowDrop { get; set; } = false;
    }

    internal class NameAttribute : Attribute
    {
        public string Name { get; set; }

        public NameAttribute(string name) => Name = name;
    }

    public static class ItemUtil
    {
        static Item DefaultItem = new Item();

        static Dictionary<string, PropertyInfo> Properties = typeof(Item).GetProperties().ToDictionary(p => p.Name, p => p);

        public static XElement ToXElement(this Item item)
        {
            var xe = new XElement("Item");
            foreach(var p in Properties.Values)
            {
                var v = p.GetValue(item);
                if ((v is string vs && string.IsNullOrEmpty(vs)) || v.Equals(p.GetValue(DefaultItem))) continue;
                xe.Add(new XAttribute(p.Name, v));
            }
            return xe;
        }

        public static Item FromXElement(this XElement xElement)
        {
            var item = new Item();
            foreach (var attr in xElement.Attributes())
            {
                if (!Properties.TryGetValue(attr.Name.LocalName, out var p)) continue;
                p.SetValue(item, p.PropertyType.Parse(attr.Value));
            }
            foreach(var elem in xElement.Elements()?.Where(e=>Properties.ContainsKey(e.Name.LocalName)))
            {
                var p = Properties[elem.Name.LocalName];
                p.SetValue(item, p.PropertyType.Parse(elem.Value));
            }
            return item;
        }
    }
}

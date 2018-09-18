using System;
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

        public static XElement ToXElement(this Item item)
        {
            var xe = new XElement("Item");
            foreach(var p in item.GetType().GetProperties())
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
            foreach(var attr in xElement.Attributes())
            {
                var p = typeof(Item).GetProperty(attr.Name.LocalName);
                if (p is null) continue;
                object value;
                if(p.PropertyType.IsEnum)
                    value = Enum.Parse(p.PropertyType, attr.Value);
                else
                    value = Convert.ChangeType(attr.Value, p.PropertyType);
                p.SetValue(item, value);
            }
            return item;
        }
    }
}

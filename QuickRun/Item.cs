using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace QuickRun
{
    public enum ItemType
    {
        Uri,
        Process,
        BackButton,
    }

    public class Item
    {
        [DisplayName("名称")]
        public string Name { get; set; } = "Node";

        [DisplayName("类型")]
        public ItemType Type { get; set; } = ItemType.Uri;

        [DisplayName("Key")]
        public string Key { get; set; } = "";

        [DisplayName("启动Uri")]
        public string Uri { get; set; } = "";

        [DisplayName("启动参数")]
        public string Arguments { get; set; } = "";

        [DisplayName("管理员权限")]
        public bool Admin { get; set; } = false;

        [DisplayName("允许拖拽")]
        public bool AllowDrop { get; set; } = false;

        [DisplayName("可用")]
        public bool Enabled { get; set; } = true;

        [DisplayName("保持开启")]
        public bool StayOpen { get; set; } = false;

        [DisplayName("样式")]
        public string Style { get; set; } = "Default";

        [DisplayName("插件路径")]
        public string Plugin { get; set; } = "";

        [DisplayName("配置路径")]
        public string DesignPath { get; set; } = "";
    }

    public static class ItemUtil
    {
        static readonly Item DefaultItem = new Item();

        static Dictionary<string, PropertyInfo> Properties = typeof(Item).GetProperties().ToDictionary(p => p.Name, p => p);

        public static XElement ToXElement(this Item item)
        {
            var xe = new XElement(nameof(Item));
            foreach (var p in Properties.Values)
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
            foreach (var elem in xElement.Elements()?.Where(e => Properties.ContainsKey(e.Name.LocalName)))
            {
                var p = Properties[elem.Name.LocalName];
                p.SetValue(item, p.PropertyType.Parse(elem.Value));
            }
            return item;
        }
    }
}

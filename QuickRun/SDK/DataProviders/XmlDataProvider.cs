using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuickRun.SDK
{
    public class XmlDataProvider : IDataProvider
    {
        public XElement Element;

        public T Get<T>(string key)
        {
            if (Element.Attribute(key) is XAttribute attr)
                return Parse<T>(attr.Value);
            return default(T);
        }

        public void Set<T>(string key, T value)
            => Element.SetAttributeValue(key, value);

        public static T Parse<T>(string value)
        {
            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), value);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public void OnAttached(IButton button)
        {
            button.Removed += (s, e) => Element?.Remove();
            if(button is IFolder folder)
                folder.ItemAdded += Folder_ItemAdded;
        }

        private void Folder_ItemAdded(object sender, EventArgs e)
        {
            if (sender is BaseButton button && button.DataProvider is XmlDataProvider xmlData)
                Element.Add(xmlData.Element);
        }
    }
}

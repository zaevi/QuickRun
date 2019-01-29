using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace QuickRun.SDK
{
    public class XmlDataStorage : IDataStorage
    {
        public IFolder RootFolder { get; set; }

        public string Path { get; set; }

        public void Save()
        {
            if(RootFolder is BaseFolder baseFolder && baseFolder.DataProvider is XmlDataProvider dataProvider)
            {
                dataProvider.Element.Save(Path);
            }
        }

        public static XmlDataStorage Load(string path)
        {
            var root = XElement.Load(path);
            var btn = GenerateButtonTree(root);
            var dataStorage = new XmlDataStorage
            {
                Path = path,
                RootFolder = btn as IFolder
            };

            return dataStorage;
        }

        private static IButton GenerateButtonTree(XElement xElement)
        {
            var type = ButtonFactory.GetButtonType(xElement.Name.ToString());
            if (type == null) return null;

            var provider = new XmlDataProvider { Element = xElement };
            var button = ButtonFactory.InstanceButton(xElement.Name.ToString(), provider);
            if (!(button is IFolder folder)) return button;

            foreach(var xe in xElement.Elements())
            {
                var btn = GenerateButtonTree(xe);
                if (btn != null)
                    folder.LoadChild(btn);
            }

            return button;
        }
    }
}

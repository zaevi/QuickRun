using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickRun.SDK
{
    public static class DataStorageFactory
    {
        public static IDataStorage InstanceXmlDataStorage(string path)
        {
            return XmlDataStorage.Load(path);
        }
    }
}

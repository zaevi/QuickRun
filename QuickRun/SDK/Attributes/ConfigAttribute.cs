using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace QuickRun.SDK
{
    public class ConfigAttribute : Attribute
    {
        public string Name { get; set; }

        public ConfigAttribute(string name)
        {
            Name = name;
        }
    }
}

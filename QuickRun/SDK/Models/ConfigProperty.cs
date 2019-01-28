using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace QuickRun.SDK
{
    public class ConfigProperty
    {
		public string Key { get; set; }
		
		public string Name { get; set; }

		public Type Type { get; set; }

		public PropertyInfo Property { get; set; }
    }
}

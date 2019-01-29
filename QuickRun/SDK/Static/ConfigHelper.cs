using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace QuickRun.SDK
{
    public static class ConfigHelper
    {
        private static Dictionary<Type, ConfigProperty[]> TypeConfigProperties = new Dictionary<Type, ConfigProperty[]>();

        public static ConfigProperty[] GetConfigProperties(Type type)
        {
            if (!(typeof(IButton).IsAssignableFrom(type))) return new ConfigProperty[] { };

            if (TypeConfigProperties.TryGetValue(type, out var array))
                return array;

            var list = new List<ConfigProperty>();
            foreach(var p in type.GetProperties())
            {
                if(p.GetCustomAttribute(typeof(ConfigAttribute)) is ConfigAttribute attr)
                {
                    list.Add(new ConfigProperty
                    {
                        Key = p.Name,
                        Name = attr.Name ?? p.Name,
                        Type = p.PropertyType,
                        Property = p
                    });
                }
            }

            array = list.ToArray();
            TypeConfigProperties[type] = array;
            return array;
        }

        public static ConfigProperty[] GetConfigProperties(this IButton button)
        {
            return GetConfigProperties(button.GetType());
        }
    }
}

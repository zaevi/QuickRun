using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickRun.Plugin;
using System.Reflection;
using System.Windows;

namespace QuickRun
{
    static class PluginManager
    {

        static HashSet<string> LoadedAssembly = new HashSet<string>();
        static Dictionary<string, Type> PluginMap = new Dictionary<string, Type>();

        public static void LoadPlugin(string path)
        {
            var assembly = Assembly.Load(path);
            if (LoadedAssembly.Contains(assembly.FullName))
                return;

            var plugins = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BasePlugin)));
            foreach(var pType in plugins)
            {
                string key = pType.GetCustomAttribute<PluginAttribute>()?.Key ?? pType.FullName;
                PluginMap[key] = pType;
            }
            LoadedAssembly.Add(assembly.FullName);

            Console.WriteLine("loaded");
        }

        public static BasePlugin ExecutePlugin(Item item, IDataObject dragData=null)
        {
            if (!PluginMap.TryGetValue(item.Key, out var pType))
                return null;
            var plugin = Activator.CreateInstance(pType) as BasePlugin;
            plugin.Arguments = item.Arguments;
            if(dragData != null)
            {
                plugin.IsDrag = true;
                if (!plugin.GetDragData(dragData))
                    return null;
            }
            plugin.Execute();
            return plugin;
        }
    }
}

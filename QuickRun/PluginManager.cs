using QuickRun.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace QuickRun
{
    static class PluginManager
    {

        static HashSet<string> LoadedAssembly = new HashSet<string>();
        static Dictionary<string, Type> PluginMap = new Dictionary<string, Type>();

        public static bool LoadPlugin(string path)
        {
            try
            {
                var assembly = Assembly.Load(path);
                if (LoadedAssembly.Contains(assembly.FullName))
                    return true;
                var plugins = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Plugin.Plugin)));
                foreach (var pType in plugins)
                {
                    string key = pType.GetCustomAttribute<PluginAttribute>()?.Key ?? ("$" + pType.FullName);
                    PluginMap[key] = pType;
                }
                LoadedAssembly.Add(assembly.FullName);
                return true;
            }
            catch { return false; }
        }

        public static Plugin.Plugin ExecutePlugin(Item item, IDataObject dragData=null)
        {
            if (!PluginMap.TryGetValue(item.Key, out var pType))
                return null;
            var plugin = Activator.CreateInstance(pType) as Plugin.Plugin;
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

using QuickRun.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                var plugins = assembly.ExportedTypes.Where(t => typeof(IPlugin).IsAssignableFrom(t));
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

        public static bool ExecutePlugin(Item item)
        {
            if (!PluginMap.TryGetValue(item.Key, out var pType))
                return false;
            var iplugin = Activator.CreateInstance(pType) as IPlugin;
            if(iplugin is Plugin.Plugin plugin)
                plugin.Arguments = item.Arguments;
            iplugin.Execute();
            return true;
        }

        public static bool ExecutePlugin(Item item, IDataObject dragData=null)
        {
            if (dragData == null) return ExecutePlugin(item);
            if (!PluginMap.TryGetValue(item.Key, out var pType))
                return false;
            var iplugin = Activator.CreateInstance(pType) as IPlugin;
            if (iplugin is Plugin.Plugin plugin)
            {
                plugin.Arguments = item.Arguments;
                plugin.IsDrag = true;
            }
            if (!(iplugin is IDragDrop ddPlugin) || !ddPlugin.GetDragData(dragData))
                return false;
            iplugin.Execute();
            return true;
        }
    }
}

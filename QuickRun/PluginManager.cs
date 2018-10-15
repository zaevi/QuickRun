using QuickRun.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace QuickRun
{
    static class PluginManager
    {

        static HashSet<string> LoadedAssembly = new HashSet<string>();
        static Dictionary<string, object> PluginMap = new Dictionary<string, object>();

        public static bool LoadPlugin(string path)
        {
            try
            {
                var assembly = Assembly.Load(path);
                if (LoadedAssembly.Contains(assembly.FullName))
                    return true;
                var name = typeof(IPlugin).Assembly.GetName().Name;
                if (assembly.GetReferencedAssemblies().Any(a=>a.Name == name))
                {
                    var plugins = assembly.ExportedTypes.Where(t => typeof(IPlugin).IsAssignableFrom(t));
                    foreach (var pType in plugins)
                    {
                        string key = pType.GetCustomAttribute<PluginAttribute>()?.Key ?? ("$" + pType.FullName);
                        PluginMap[key] = pType;
                    }
                }
                else
                {
                    var types = assembly.ExportedTypes.Where(t => t.Name.EndsWith(@"QuickRun") || t.Name.EndsWith(@"QuickRunPlugin"));
                    foreach(var pType in types)
                    {
                        var method = pType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
                        if (method is null) continue;
                        var pObj = new NonStandardPlugin(method);
                        var key = pType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                            ?? pType.FullName.Substring(0, pType.FullName.LastIndexOf("QuickRun"));
                        key = "$" + key.TrimStart('$');
                        PluginMap[key] = pObj;
                    }
                }
                LoadedAssembly.Add(assembly.FullName);
                return true;
            }
            catch { return false; }
        }

        public static bool ExecutePlugin(Item item)
        {
            if (!PluginMap.TryGetValue(item.Key, out var pObj))
                return false;
            IPlugin iplugin;
            if (pObj is Type) iplugin = Activator.CreateInstance(pObj as Type) as IPlugin;
            else iplugin = pObj as IPlugin;
            if(iplugin is Plugin.Plugin plugin)
                plugin.Arguments = item.Arguments;
            iplugin.Execute();
            return true;
        }

        public static bool ExecutePlugin(Item item, IDataObject dragData=null)
        {
            if (dragData == null) return ExecutePlugin(item);
            if (!PluginMap.TryGetValue(item.Key, out var pObj))
                return false;
            IPlugin iplugin;
            if (pObj is Type) iplugin = Activator.CreateInstance(pObj as Type) as IPlugin;
            else iplugin = pObj as IPlugin;
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

    class NonStandardPlugin : FileDropPlugin
    {
        MethodInfo Method = null;

        public NonStandardPlugin(MethodInfo method)
        {
            Method = method;
        }

        public override void Execute()
        {
            Method?.Invoke(null, null);
        }
    }
}

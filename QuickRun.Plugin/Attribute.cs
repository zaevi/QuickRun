using System;

namespace QuickRun.Plugin
{
    public class PluginAttribute : Attribute
    {
        /// <summary>
        /// 用于展示插件功能名
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// 用于Item使用Key来调用功能
        /// </summary>
        public string Key { get; set; } = null;

        public PluginAttribute(string key, string name)
        {
            Key = "$" + key.TrimStart('$');
            Name = name;
        }

        public PluginAttribute(string key)
        {
            Key = "$" + key.TrimStart('$');
        }
    }
}

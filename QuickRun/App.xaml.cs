using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QuickRun
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static bool? isPluginSupported;
        public static bool IsPluginSupported { get {
                if (!isPluginSupported.HasValue)
                    isPluginSupported = System.IO.File.Exists("QuickRun.Plugin.dll");
                return isPluginSupported.Value;
            } }
    }
}

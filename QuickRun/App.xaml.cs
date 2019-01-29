using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using QuickRun.SDK;
using QuickRun.Windows;
using QuickRun.Windows.Components;

namespace QuickRun
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// QuickRun App实例
        /// </summary>
        public new static App Current => Application.Current as App;

        private static bool? isPluginSupported;
        public static bool IsPluginSupported { get {
                if (!isPluginSupported.HasValue)
                    isPluginSupported = System.IO.File.Exists("QuickRun.Plugin.dll");
                return isPluginSupported.Value;
            } }

        public Navigator Navigator { get; set; }

        public IDataStorage DataStorage { get; set; }

        /// <summary>
        /// QuickRun 主窗口
        /// </summary>
        public new Windows.Main MainWindow => base.MainWindow as Windows.Main;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            LoadDataStorage("design.xml");
        }

        public void LoadDataStorage(string path)
        {
            DataStorage = DataStorageFactory.InstanceXmlDataStorage(path);
            Navigator = new Navigator { RootFolder = DataStorage.RootFolder, Main = MainWindow };
            Navigator.SetFolder(Navigator.RootFolder);
        }
    }
}

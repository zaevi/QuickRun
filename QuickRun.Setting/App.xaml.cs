using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace QuickRun.Setting
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith(nameof(QuickRun)+","))
            {
                var msg = "无法找到QuickRun主程序, 请移动编辑器至主程序目录下, 或按\"确定\"指定主程序位置";
                if (MessageBox.Show(msg, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    var dialog = new System.Windows.Forms.OpenFileDialog()
                    {
                        FileName = "QuickRun.exe",
                        Filter = "QuickRun主程序|*.exe"
                    };
                    if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            var assembly = Assembly.LoadFile(dialog.FileName);
                            if (assembly.GetName().Name == nameof(QuickRun) && assembly.GetType("QuickRun.Item") != null)
                                return assembly;
                            throw new Exception();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("程序集加载失败!");
                        }
                    }
                }
                Environment.Exit(-1);
            }
            return args.RequestingAssembly;
        }
    }
}

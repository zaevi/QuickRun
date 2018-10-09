using Microsoft.Win32;
using QuickRun.Plugin;
using System.Windows.Forms;

namespace QuickRun.Extension
{
    [Plugin("$SetStartup", "设置开机启动")]
    public class SetStartupPlugin : IPlugin
    {
        const string StartupSubKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

        const string AppName = @"QuickRun";

        readonly string AppPath = $"\"{typeof(App).Assembly.Location}\" -h";

        public void Execute()
        {
            var key = Registry.CurrentUser.OpenSubKey(StartupSubKey, true);

            if(key.GetValue(AppName) == null)
            {
                key.SetValue(AppName, AppPath);
                MessageBox.Show("已设为开机启动", "QuickRun");
            }
            else
            {
                key.DeleteValue(AppName, false);
                MessageBox.Show("已取消开机启动", "QuickRun");
            }
        }
    }
}

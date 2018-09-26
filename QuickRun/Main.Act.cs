using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Markup;
using System.Collections;
using System.Diagnostics;
using System.Xml.Linq;

namespace QuickRun
{
    partial class Main : Window
    {

        public void Action_RunAction(string key, string arguments=null)
        {
            if (Map.TryGetValue(key, out var item))
            {
                var startInfo = new ProcessStartInfo()
                {
                    FileName = Environment.ExpandEnvironmentVariables(item.Uri),
                    Arguments = string.Join(" ", item.Arguments, arguments),
                };
                if (File.Exists(startInfo.FileName))
                    startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(startInfo.FileName);
                try
                {
                    Process.Start(startInfo);
                    if(!item.StayOpen) this.Hide();
                }
                catch(Exception e)
                {
                    MessageBox.Show($"{e.GetType().Name}:\n{e.Message}", "Error");
                }
            }
            
        }

        Stack<string> FolderHistory = new Stack<string>();

        public void Action_ShowFolder(string key, bool back=false)
        {
            if(back)
            {
                key = FolderHistory.Pop();
            }
            else
            {
                FolderHistory.Push(CurrentFolder);
            }
            if (mainPanel.Children.Count > 1)
                mainPanel.Children.RemoveAt(1);
            mainPanel.Children.Add(Folder[key]);
            CurrentFolder = key;
            backBtn.Visibility = key == "#0" ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;

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
                    startInfo.WorkingDirectory = Path.GetDirectoryName(startInfo.FileName);
                if (item.Admin)
                    startInfo.Verb = "RunAs";
                try
                {
                    Process.Start(startInfo);
                    if(!item.StayOpen) this.Hide();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"{e.GetType().Name}:\n{e.Message}", "Error");
                }
            }
            
        }

        public void Action_RunPlugin(string key, IDataObject dragData = null)
        {
            if(Map.TryGetValue(key, out var item))
            {
                try
                {
                    var result = PluginManager.ExecutePlugin(item, dragData);
                    if (result != null && result.Success && !item.StayOpen) Hide();
                }
                catch (Exception e)
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
                if (FolderHistory.Count == 0) return;
                key = FolderHistory.Pop();
            }
            else if(CurrentFolder != null)
            {
                FolderHistory.Push(CurrentFolder);
            }
            if (mainPanel.Children.Count > 1)
                mainPanel.Children.RemoveAt(1);
            mainPanel.Children.Add(Folder[key]);
            title.Content = Folder[key].Name;
            CurrentFolder = key;
            backBtn.Visibility = key == "#0" ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}

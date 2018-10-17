using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                    var executed = PluginManager.ExecutePlugin(item, dragData);
                    if (executed && !item.StayOpen) Hide();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"{e.GetType().Name}:\n{e.Message}", "Error");
                }
            }
        }

        Stack<string> FolderHistory = new Stack<string>();
        Dictionary<string, int> FocusedIndex = new Dictionary<string, int>();

        public void Action_ShowFolder(string key, bool back=false)
        {
            if (CurrentFolder != null && Keyboard.FocusedElement is Button btn)
            {
                var idx = Folder[CurrentFolder].Children.IndexOf(btn);
                if (idx >= 0)
                    FocusedIndex[CurrentFolder] = idx;
            }

            if (back)
            {
                if (FolderHistory.Count == 0) return;
                key = FolderHistory.Pop();
            }
            else if(CurrentFolder != null)
            {
                FolderHistory.Push(CurrentFolder);
            }
            if (mainPanel.Children.Count > 1) // TODO
                mainPanel.Children.RemoveAt(1);
            mainPanel.Children.Add(Folder[key]);
            title.Content = Folder[key].Name;
            CurrentFolder = key;
            backBtn.Visibility = key == "#0" ? Visibility.Collapsed : Visibility.Visible;

            if (FocusedIndex.TryGetValue(key, out var index) && index < Folder[key].Children.Count)
                Keyboard.Focus(Folder[key].Children[index]);
        }
    }
}

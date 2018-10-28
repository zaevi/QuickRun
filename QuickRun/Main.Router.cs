using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Forms = System.Windows.Forms;

namespace QuickRun
{
    partial class Main : Window
    {
        void RouteItem(Item item, IDataObject data=null)
        {
            if (ItemFolder.ContainsKey(item))
                ShowFolder(item);
            else if (item.Type == ItemType.BackButton)
                ShowFolder(null, true);
            else if (item.Key.StartsWith("$"))
                ExecutePluginItem(item, data);
            else
                ExecuteProcessItem(item, data);
        }

        void ExecuteProcessItem(Item item, IDataObject data=null)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = Environment.ExpandEnvironmentVariables(item.Uri),
                Arguments = item.Arguments
            };
            if (data != null)
            {
                if (data.GetFormats().Contains("FileName"))
                {
                    var files = (string[])data.GetData(DataFormats.FileDrop);
                    startInfo.Arguments += " " + files;
                }
            }
            if (File.Exists(startInfo.FileName))
                startInfo.WorkingDirectory = Path.GetDirectoryName(startInfo.FileName);
            if (item.Admin)
                startInfo.Verb = "RunAs";
            try
            {
                Process.Start(startInfo);
                if (!item.StayOpen) this.Hide();
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.GetType().Name}:\n{e.Message}", "Error");
            }
            
        }

        void ExecutePluginItem(Item item, IDataObject data=null)
        {
            try
            {
                var executed = PluginManager.ExecutePlugin(item, data);
                if (executed && !item.StayOpen) Hide();
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.GetType().Name}:\n{e.Message}", "Error");
            }
        }



        void ShowFolder(Item item, bool back = false)
        {
            if (back)
            {
                if (ItemFolderHistory.Count == 0) return;
                item = ItemFolderHistory.Pop();
            }
            else if (CurrentItem != null)
            {
                ItemFolderHistory.Push(CurrentItem);
            }

            var items = ItemFolder[item];
            itemsControl.ItemsSource = items;
            title.Content = (item != RootItem || (item.Name != "Node")) ? item.Name : "QuickRun";
            CurrentItem = item;
            backBtn.Visibility = CurrentItem == RootItem ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}

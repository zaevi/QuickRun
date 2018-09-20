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
            if (Map.TryGetValue(key, out string uri))
            {
                uri = Environment.ExpandEnvironmentVariables(uri);
                Process.Start(uri, arguments);
            }
            this.Hide();
        }

        public void Action_LoadStyles(string stylePath)
        {
            using (var fs = File.OpenRead(stylePath))
            {
                Resources = XamlReader.Load(fs) as ResourceDictionary;
            }
        }

        public void Action_LoadItems(string itemPath, string mapPath)
        {
            Folder.Clear();
            using (var fs = File.OpenRead(itemPath))
            {
                var panel = XamlReader.Load(fs) as StackPanel;
                foreach (var btn in panel.FindVisualChildren<Button>())
                {
                    btn.Click += Button_Click;
                    if(btn.AllowDrop)
                    {
                        //btn.PreviewDragOver += Btn_PreviewDragOver;
                        btn.PreviewDrop += Btn_PreviewDrop;
                    }
                }
                foreach (Panel sp in panel.Children)
                    Folder.Add(sp.Tag.ToString(), sp);
                panel.Children.Clear();
            }

            var xe = XElement.Load(mapPath);
            foreach (var action in xe.Elements("Action"))
            {
                switch (action.GetAttribute("Type"))
                {
                    case "Uri":
                        Map.Add(action.GetAttribute("Key"), action.GetAttribute("Uri"));
                        break;
                }
            }

            Action_ShowFolder("#0");
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

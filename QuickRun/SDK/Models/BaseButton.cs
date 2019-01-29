using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace QuickRun.SDK
{
    public class BaseButton : IButton, INotifyPropertyChanged
    {
        private IDataProvider _dataProvider;
        public IDataProvider DataProvider { get => _dataProvider; set { _dataProvider = value; _dataProvider.OnAttached(this); } }

        public T GetValue<T>(string key)
            => DataProvider.Get<T>(key);

        public string GetValue(string key)
            => DataProvider.Get<string>(key);

        public void SetValue<T>(string key, T value)
        {
            DataProvider.Set(key, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
        }

        [Config("Id")]
        public string Id
        {
            get => GetValue<string>("Id");
            set => SetValue("Id", value);
        }

        [Config("名称")]
        public string Name {
            get => GetValue<string>("Name");
            set => SetValue("Name", value);
        }

        public System.Windows.Controls.Button Control = null;

        public event EventHandler Created;
        public event EventHandler Initialized;
        public event EventHandler Modified;
        public event EventHandler Removed;
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnInitialized(object sender, EventArgs e)
        {
            if (!(sender is System.Windows.Controls.Button button)) return;
            
            Control = button;
            button.Click += OnClick;
        }

        public virtual void OnClick(object sender, RoutedEventArgs e) { }
    }
}

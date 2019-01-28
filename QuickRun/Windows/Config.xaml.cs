using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QuickRun.SDK;

namespace QuickRun.Windows
{
    /// <summary>
    /// Config.xaml 的交互逻辑
    /// </summary>
    public partial class Config : Window
    {
        private Type _configType;

        public Type ConfigType
        {
            get => _configType;
            set { BuildConfig(value); }
        }

        public new string Title
        {
            get => base.Title;
            set { base.Title = value; titleLabel.Content = value; }
        }

        public Config()
        {
            InitializeComponent();
            titleBar.MouseLeftButtonDown += (s, me) => DragMove();
            closeBtn.Click += (s, e) =>
            {
                DialogResult = false;
                this.Close();
            };
        }

        private void BuildConfig(Type type, BindingMode bindingMode = BindingMode.Default)
        {
            if (_configType == type) return;
            _configType = type;

            var configProperties = ConfigHelper.GetConfigProperties(type);

            for (int i = 1; i < configProperties.Length; i++)
                dataGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });

            for (int i = 0; i < configProperties.Length; i++)
            {
                var property = configProperties[i];

                var label = new Label() { Content = property.Name, ToolTip = property.Key };
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);
                dataGrid.Children.Add(label);

                var element = AddPropertyElement(property, bindingMode);
                element.VerticalAlignment = VerticalAlignment.Center;
                element.Margin = new Thickness(5);
                Grid.SetRow(element, i);
                Grid.SetColumn(element, 2);
                dataGrid.Children.Add(element);
            }
        }

        private FrameworkElement AddPropertyElement(ConfigProperty property, BindingMode bindingMode)
        {
            FrameworkElement element = null;
            var type = property.Type;
            var binding = new Binding(property.Key) { Mode = bindingMode };
            if (type == typeof(string))
            {
                element = new TextBox();
                element.SetBinding(TextBox.TextProperty, binding);
                
            }
            else if (type == typeof(bool))
            {
                element = new CheckBox();
                element.SetBinding(CheckBox.IsCheckedProperty, binding);
            }
            else if (type.IsEnum)
            {
                element = new ComboBox() { ItemsSource = Enum.GetValues(type) };
                element.SetBinding(ComboBox.SelectedValueProperty, binding);
            }
            return element;
        }
    }
}

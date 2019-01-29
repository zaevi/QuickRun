using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickRun.SDK
{
    public static class ButtonFactory
    {
        private static Type[] s_localButtonTypes;

        public static Type[] LocalButtonTypes
        {
            get
            {
                s_localButtonTypes = s_localButtonTypes 
                    ?? typeof(ButtonFactory).Assembly.GetTypes().Where(t => typeof(IButton).IsAssignableFrom(t)).ToArray();
                return s_localButtonTypes;
            }
        }

        public static IButton InstanceButton(Type type, IDataProvider dataProvider)
        {
            if (!typeof(IButton).IsAssignableFrom(type)) return null;

            IButton button = (IButton)Activator.CreateInstance(type);

            if (button is BaseButton baseButton)
            {
                if (dataProvider != null)
                    baseButton.DataProvider = dataProvider;
                else
                    baseButton.DataProvider = new MemoryDataProvider();
            }

            return button;
        }

        public static IButton InstanceButton(string typeName, IDataProvider dataProvider)
        {
            var type = GetButtonType(typeName);
            if (type != null)
            {
                return InstanceButton(type, dataProvider);
            }
            else
            {
                var msgButton = InstanceButton(typeof(MessageBoxButton), dataProvider) as MessageBoxButton;
                msgButton.Message = $"Button type {typeName} doesn't exist.";
                return msgButton;
            }
        }

        public static Type GetButtonType(string typeName)
        {
            var type = LocalButtonTypes.Where(t=>t.Name == typeName).FirstOrDefault();
            return type;
        }
    }
}

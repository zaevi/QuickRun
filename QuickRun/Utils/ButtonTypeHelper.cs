using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickRun.SDK;

namespace QuickRun.Utils
{
    internal static class ButtonTypeHelper
    {
        public static Type[] GetButtonTypes()
        {
            typeof(ButtonTypeHelper).Assembly.GetTypes().Where(t => t.IsAssignableFrom(typeof(IButton)));
            return null;
        }
    }
}

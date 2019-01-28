using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickRun.SDK
{
    public interface IDataProvider
    {
        void Set<T>(string key, T value);

        T Get<T>(string key);

        void OnAttached(IButton button);
    }
}

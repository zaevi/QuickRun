using System.Collections.Generic;

namespace QuickRun.SDK
{
    public class MemoryDataProvider : IDataProvider
    {
        public Dictionary<string, object> Dict = new Dictionary<string, object>();

        public T Get<T>(string key)
        {
            if (Dict.TryGetValue(key, out var value))
                return (T)value;
            return default(T);
        }

        public void Set<T>(string key, T value)
        {
            Dict[key] = value;
        }

        public void OnAttached(IButton button) { }
    }
}

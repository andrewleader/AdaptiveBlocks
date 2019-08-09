using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Visualizer.Uwp.Settings
{
    public sealed class SettingsValueHelper<T>
    {
        public string Key { get; private set; }
        private T _defaultValue;

        public SettingsValueHelper(string key, T defaultValue)
        {
            Key = key;
            _defaultValue = defaultValue;
        }

        public T Value
        {
            get
            {
                object value;

                if (typeof(T).GetTypeInfo().IsEnum)
                {
                    if (OverallSettings._values.TryGetValue(Key, out value) && value is string)
                    {
                        try
                        {
                            return (T)Enum.Parse(typeof(T), value as string);
                        }
                        catch { return _defaultValue; }
                    }
                    return _defaultValue;
                }

                if (OverallSettings._values.TryGetValue(Key, out value) && value is T)
                    return (T)value;

                return _defaultValue;
            }

            set
            {
                if (typeof(T).GetTypeInfo().IsEnum)
                {
                    OverallSettings._values[Key] = value.ToString();
                }
                else
                {
                    OverallSettings._values[Key] = value;
                }
            }
        }
    }
}

using SysCommand.Compatibility;
using System;
using System.Collections.Generic;

namespace SysCommand.ConsoleApp
{
    public class ItemCollection : Dictionary<object, object>
    {
        public T Get<T>()
        {
            return this.Get<T>(typeof(T));
        }

        public T Get<T>(object key)
        {
            var obj = this[key];
            return this.Convert<T>(obj);
        }

        public T GetOrCreate<T>()
        {
            return this.GetOrCreate<T>(typeof(T));
        }

        public T GetOrCreate<T>(object key)
        {
            if (this.ContainsKey(key))
                return this.Get<T>(key);

            var instance = Activator.CreateInstance<T>();
            this.Add(key, instance);
            return instance;
        }

        private T Convert<T>(object obj)
        {
            if (obj != null)
                return (T)obj;
            else if (!typeof(T).IsValueType())
                return default(T);

            throw new Exception("Can not convert a null value to a struct type.");
        }
    }
}

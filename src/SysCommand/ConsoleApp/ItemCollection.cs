using SysCommand.Compatibility;
using System;
using System.Collections.Generic;

namespace SysCommand.ConsoleApp
{
    public class ItemCollection : Dictionary<object, object>
    {
        /// <summary>
        /// Returns the first element of the T type, if you can't find so returns null to complex types and Exception for struct .
        /// </summary>
        /// <typeparam name="T">Type to get</typeparam>
        /// <returns></returns>
        public T Get<T>()
        {
            return this.Get<T>(typeof(T));
        }

        /// <summary>
        /// Returns the first element of informed key return behavior is the same as the above method.
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="key">Key to find</param>
        /// <returns>Instance of T</returns>
        public T Get<T>(object key)
        {
            var obj = this[key];
            return this.Convert<T>(obj);
        }

        /// <summary>
        /// If exists, returns the first element of type T or creates a new instance via reflection where type T is the key.
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <returns>Instance of T</returns>
        public T GetOrCreate<T>()
        {
            return this.GetOrCreate<T>(typeof(T));
        }

        /// <summary>
        /// If exists, returns the first element of informed key or creates a new instance via reflection.
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="key">Key to find</param>
        /// <returns>Instance of T</returns>
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

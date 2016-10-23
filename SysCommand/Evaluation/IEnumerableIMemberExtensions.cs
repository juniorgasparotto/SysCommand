using SysCommand.Parsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SysCommand.Evaluation
{
    public static class IEnumerableIMemberExtensions 
    {
        public static IEnumerable<IMember> WithName(this IEnumerable<IMember> list, string name)
        {
            return list.Where(f => f.Name == name);
        }

        public static IEnumerable<T> WithSource<T>(this IEnumerable<T> list) where T : IMember
        {
            return list.WithSource(typeof(T));
        }

        public static IEnumerable<T> WithSource<T>(this IEnumerable<T> list, Type type) where T : IMember
        {
            return list.Where(f => f.Source != null && f.Source.GetType() == type);
        }

        public static IEnumerable<T> WithValue<T>(this IEnumerable<T> list, object value) where T : IMember
        {
            // prevent comparation that '==' dosen't work
            return list.Where(f => f.Value == value || (f.Value != null && value != null && f.Value.Equals(value)));
        }

        public static IEnumerable<TFilter> With<TFilter>(this IEnumerable<IMember> list) where TFilter : IMember
        {
            return list.With<TFilter>(f => f is TFilter);
        }

        public static IEnumerable<TFilter> With<TFilter>(this IEnumerable<IMember> list, Func<TFilter, bool> expression) where TFilter : IMember
        {
            return list.Where(f => f is TFilter).Cast<TFilter>().Where(expression);
        }

        public static void Invoke(this IEnumerable<IMember> list, Action<IMember> onInvoke)
        {
            foreach (var m in list)
            {
                if (onInvoke == null)
                    m.Invoke();
                else
                    onInvoke(m);
            }
        }

        public static object GetValue(this IEnumerable<IMember> list, int index = 0)
        {
            var value = list.ElementAtOrDefault(index);
            if (value != null)
                return value.Value;
            return null;
        }

        public static TValue GetValue<TValue>(this IEnumerable<IMember> list, int index = 0)
            where TValue : class
        {
            var value = list.ElementAtOrDefault(index);
            if (value != null && value.Value is TValue)
                return (TValue)value.Value;
            return null;
        }

        public static TValue? GetNullableValue<TValue>(this IEnumerable<IMember> list, int index = 0)
            where TValue : struct
        {
            var value = list.ElementAtOrDefault(index);
            if (value != null && value.Value is TValue)
                return (TValue)value.Value;
            return null;
        }
    }
}

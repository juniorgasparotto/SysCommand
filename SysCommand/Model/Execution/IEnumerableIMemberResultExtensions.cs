using System;
using System.Collections.Generic;
using System.Linq;

namespace SysCommand.Execution
{
    public static class IEnumerableIMemberResultExtensions
    {
        public static IEnumerable<IMemberResult> WithName(this IEnumerable<IMemberResult> list, string name)
        {
            return list.Where(f => f.Name == name);
        }

        public static IEnumerable<T> WithTarget<T>(this IEnumerable<T> list) where T : IMemberResult
        {
            return list.WithTarget(typeof(T));
        }

        public static IEnumerable<T> WithTarget<T>(this IEnumerable<T> list, Type type) where T : IMemberResult
        {
            return list.Where(f => f.Target != null && f.Target.GetType() == type);
        }

        public static IEnumerable<T> WithValue<T>(this IEnumerable<T> list, object value) where T : IMemberResult
        {
            // prevent comparation that '==' dosen't work
            return list.Where(f => f.Value == value || (f.Value != null && value != null && f.Value.Equals(value)));
        }

        public static IEnumerable<TFilter> With<TFilter>(this IEnumerable<IMemberResult> list) where TFilter : IMemberResult
        {
            return list.With<TFilter>(f => f is TFilter);
        }

        public static IEnumerable<TFilter> With<TFilter>(this IEnumerable<IMemberResult> list, Func<TFilter, bool> expression) where TFilter : IMemberResult
        {
            return list.Where(f => f is TFilter).Cast<TFilter>().Where(expression);
        }

        public static void Invoke(this IEnumerable<IMemberResult> list, Action<IMemberResult> onInvoke)
        {
            foreach (var m in list)
            {
                if (onInvoke == null)
                    m.Invoke();
                else
                    onInvoke(m);
            }
        }

        public static object GetValue(this IEnumerable<IMemberResult> list, int index = 0)
        {
            var value = list.ElementAtOrDefault(index);
            if (value != null)
                return value.Value;
            return null;
        }

        public static TValue GetValue<TValue>(this IEnumerable<IMemberResult> list, int index = 0)
            where TValue : class
        {
            var value = list.ElementAtOrDefault(index);
            if (value != null && value.Value is TValue)
                return (TValue)value.Value;
            return null;
        }

        public static TValue? GetNullableValue<TValue>(this IEnumerable<IMemberResult> list, int index = 0)
            where TValue : struct
        {
            var value = list.ElementAtOrDefault(index);
            if (value != null && value.Value is TValue)
                return (TValue)value.Value;
            return null;
        }
    }
}

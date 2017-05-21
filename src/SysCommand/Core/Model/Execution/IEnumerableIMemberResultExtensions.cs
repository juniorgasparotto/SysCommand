using System;
using System.Collections.Generic;
using System.Linq;

namespace SysCommand.Execution
{
    /// <summary>
    /// Extension with some search help functions from IMemberResult
    /// </summary>
    public static class IEnumerableIMemberResultExtensions
    {
        /// <summary>
        /// Search for a member by their name
        /// </summary>
        /// <param name="list">List of members</param>
        /// <param name="name">Member name</param>
        /// <returns>List of IMemberResult</returns>
        public static IEnumerable<IMemberResult> WithName(this IEnumerable<IMemberResult> list, string name)
        {
            return list.Where(f => f.Name == name);
        }

        /// <summary>
        /// Search for a member by their Target
        /// </summary>
        /// <typeparam name="T">Type of target</typeparam>
        /// <param name="list">List of members</param>
        /// <returns>List of T</returns>
        public static IEnumerable<T> WithTarget<T>(this IEnumerable<T> list) where T : IMemberResult
        {
            return list.WithTarget(typeof(T));
        }

        /// <summary>
        /// Search for a member by their Target
        /// </summary>
        /// <typeparam name="T">Type of target</typeparam>
        /// <param name="list">List of members</param>
        /// <param name="type">Type of target</param>
        /// <returns>List of T</returns>
        public static IEnumerable<T> WithTarget<T>(this IEnumerable<T> list, Type type) where T : IMemberResult
        {
            return list.Where(f => f.Target != null && f.Target.GetType() == type);
        }

        /// <summary>
        /// Search for a member by their Value
        /// </summary>
        /// <typeparam name="T">Type of target</typeparam>
        /// <param name="list">List of members</param>
        /// <param name="value">Member value</param>
        /// <returns>List of IMemberResult</returns>
        public static IEnumerable<T> WithValue<T>(this IEnumerable<T> list, object value) where T : IMemberResult
        {
            // prevent comparation that '==' dosen't work
            return list.Where(f => f.Value == value || (f.Value != null && value != null && f.Value.Equals(value)));
        }

        /// <summary>
        /// Search for a member by specific Type
        /// </summary>
        /// <typeparam name="TFilter">Member type</typeparam>
        /// <param name="list">List of members</param>
        /// <returns>List of TFilter</returns>
        public static IEnumerable<TFilter> With<TFilter>(this IEnumerable<IMemberResult> list) where TFilter : IMemberResult
        {
            return list.With<TFilter>(f => f is TFilter);
        }

        /// <summary>
        /// Search for a member by specific Type and expression
        /// </summary>
        /// <typeparam name="TFilter">Member type</typeparam>
        /// <param name="list">List of members</param>
        /// <param name="expression">Filter expression</param>
        /// <returns>List of TFilter</returns>
        public static IEnumerable<TFilter> With<TFilter>(this IEnumerable<IMemberResult> list, Func<TFilter, bool> expression) where TFilter : IMemberResult
        {
            return list.Where(f => f is TFilter).Cast<TFilter>().Where(expression);
        }

        /// <summary>
        /// Get member value without exception
        /// </summary>
        /// <param name="list">List of members</param>
        /// <param name="index">Position in list</param>
        /// <returns>List of objects</returns>
        public static object GetValue(this IEnumerable<IMemberResult> list, int index = 0)
        {
            var value = list.ElementAtOrDefault(index);
            if (value != null)
                return value.Value;
            return null;
        }

        /// <summary>
        /// Get member value without exception
        /// </summary>
        /// <typeparam name="TValue">Type to cast</typeparam>
        /// <param name="list">List of members</param>
        /// <param name="index">Position in list</param>
        /// <returns>List of TValue</returns>
        public static TValue GetValue<TValue>(this IEnumerable<IMemberResult> list, int index = 0)
            where TValue : class
        {
            var value = list.ElementAtOrDefault(index);
            if (value != null && value.Value is TValue)
                return (TValue)value.Value;
            return null;
        }

        /// <summary>
        /// Get member value without exception, but only to nullable structs
        /// </summary>
        /// <typeparam name="TValue">Type to cast</typeparam>
        /// <param name="list">List of members</param>
        /// <param name="index">Position in list</param>
        /// <returns>List of TValue</returns>
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

using SysCommand.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SysCommand
{
    public sealed class Result<T> : IEnumerable<T> where T : IMember
    {
        private List<IMember> all = new List<IMember>();

        public IMember this[int index]
        {
            get
            {
                return all[index];
            }
        }

        public int Count
        {
            get
            {
                return all.Count;
            }
        }

        public Result()
        {

        }

        private Result(IEnumerable<IMember> results)
        {
            this.AddRange(results);
        }

        public Result(IEnumerable<T> results)
        {
            this.AddRange(results.Cast<IMember>());
        }

        public Result<T> WithName(string name)
        {
            return new Result<T>(this.all.Where(f => f.Name == name));
        }

        //public Result<T> WithAlias(string alias)
        //{
        //    return new Result<T>(this.all.Where(f => f.Alias == alias));
        //}

        //public Result<T> WithSourceIs<TSource>()
        //{
        //    return new Result<T>(this.all.Where(f => f is TSource));
        //}

        public Result<T> WithSource<TSource>()
        {
            return this.WithSource(typeof(TSource));
        }

        public Result<T> WithSource(Type type)
        {
            return new Result<T>(this.all.Where(f => f.Source != null && f.Source.GetType() == type));
        }

        public Result<T> WithValue(object value)
        {
            // prevent comparation that '==' dosen't work
            return new Result<T>(this.all.Where(f => f.Value == value || (f.Value != null && value != null && f.Value.Equals(value))));
        }

        public Result<T> With()
        {
            return With<T>();
        }

        public Result<T> With(Func<T, bool> expression)
        {
            return With<T>(expression);
        }

        public Result<TFilter> With<TFilter>() where TFilter : IMember
        {
            return new Result<TFilter>(this.all.Where(f => f is TFilter));
        }

        public Result<TFilter> With<TFilter>(Func<TFilter, bool> expression) where TFilter : IMember
        {
            return new Result<TFilter>(this.all.Where(f => f is TFilter && expression((TFilter)f)));
        }

        public Result<T> Invoke(Action<IMember> onInvoke)
        {
            foreach (var m in this.all)
            {
                if (onInvoke == null)
                    m.Invoke();
                else
                    onInvoke(m);
            }
            return this;
        }

        public object GetValue(int index = 0)
        {
            var value = this.all.ElementAtOrDefault(index);
            if (value != null)
                return value.Value;
            return null;
        }

        public TValue GetValue<TValue>(int index = 0)
            where TValue : class
        {
            var value = this.all.ElementAtOrDefault(index);
            if (value != null && value.Value is TValue)
                return (TValue)value.Value;
            return null;
        }

        public TValue? GetNullableValue<TValue>(int index = 0)
            where TValue : struct
        {
            var value = this.all.ElementAtOrDefault(index);
            if (value != null && value.Value is TValue)
                return (TValue)value.Value;
            return null;
        }

        public void Add(IMember result)
        {
            this.all.Add(result);
        }
        
        public void AddRange(IEnumerable<IMember> result)
        {
            this.all.AddRange(result);
        }

        public void Insert(int index, IMember result)
        {
            this.all.Insert(index, result);
        }

        public void InsertRange(int index, IEnumerable<IMember> result)
        {
            this.all.InsertRange(index, result);
        }

        public Result<Method> WithValidMethods()
        {
            return this.With<Method>(f => f.ActionMapped.MappingStates.HasFlag(ActionMappingState.Valid));
        }

        public Result<Property> WithValidProperties()
        {
            return this.With<Property>(f => f.ArgumentMapped.MappingStates.HasFlag(ArgumentMappingState.Valid));
        }

        public IEnumerator GetEnumerator()
        {
            return all.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return all.Cast<T>().GetEnumerator();
        }
    }
}

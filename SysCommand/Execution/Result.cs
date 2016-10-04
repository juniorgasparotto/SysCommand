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

        internal Result()
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

        public Result<T> WithAlias(string alias)
        {
            return new Result<T>(this.all.Where(f => f.Alias == alias));
        }

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
            return new Result<T>(this.all.Where(f => f.Source.GetType() == type));
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

        public Result<T> Invoke(/*int? priority = null*/)
        {
            //if (priority == null)
            //    this.all.ForEach(f => f.Invoke());
            //else
            //    this.all
            //        .Where(f => f.InvokePriority == priority.Value)
            //        .ToList()
            //        .ForEach(f => f.Invoke());
            this.all.ForEach(f => f.Invoke());
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

        internal void Add(IMember result)
        {
            this.all.Add(result);
        }


        internal void AddRange(IEnumerable<IMember> result)
        {
            this.all.AddRange(result);
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

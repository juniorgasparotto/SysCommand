﻿#if !(NETSTANDARD1_6)
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using SysCommand.Mapping;
using SysCommand.Helpers;
using SysCommand.ConsoleApp.View.TemplatesGenerator.Razor;
#endif

using SysCommand.Execution;
using System.Linq;
using SysCommand.ConsoleApp.View.TemplatesGenerator.T4;
using SysCommand.Parsing;

namespace SysCommand.ConsoleApp
{
    public abstract class Command : CommandBase
    {
        public App App { get; internal set; }

        public ArgumentParsed GetArgument(string name)
        {
            // return all properties results for this instance
            var allPropertiesResult = this.ExecutionScope.ExecutionResult.Results.With<PropertyResult>();
            var thisMethodForEachLevel = allPropertiesResult.Where(m => m.Target == this).ToList();

            return thisMethodForEachLevel.LastOrDefault(f => f.Name == name)?.ArgumentParsed;
        }
        
        public string ViewT4<TTemplate>()
        {
            return T4Helper.Execute<TTemplate, object>(null);
        }

        public string ViewT4<TTemplate, TModel>(TModel model = default(TModel))
        {
            return T4Helper.Execute<TTemplate, TModel>(model);
        }

#if !(NETSTANDARD1_6)
        [MethodImpl(MethodImplOptions.NoInlining)]
        public ActionMap CurrentActionMap()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            var currentMethod = (MethodInfo)sf.GetMethod();
            return this.ExecutionScope.ParseResult.Maps.GetCommandMap(this.GetType()).Methods.FirstOrDefault(m => ReflectionHelper.MethodsAreEquals(m.Method, currentMethod));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ActionParsed GetAction()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            var currentMethod = (MethodInfo)sf.GetMethod();

            // return this method (eg: save) in all levels
            // -> save 1 2 3 save 4 5 6 save 7 8 9
            var allMethodResult = this.ExecutionScope.ExecutionResult.Results.With<MethodResult>();
            var thisMethodForEachLevel = allMethodResult.Where(m => ReflectionHelper.MethodsAreEquals(m.ActionParsed.ActionMap.Method, currentMethod)).ToList();

            //  1.0) f.IsInvoked = false: save 1 2 3 -> FIRST: IsInvoked = FALSE
            //  1.1) f.IsInvoked = false: save 4 5 6 -> NO
            //  1.2) f.IsInvoked = false: save 7 8 9 -> NO
            //  2.0) f.IsInvoked = true:  save 1 2 3 -> NO
            //  2.1) f.IsInvoked = false: save 4 5 6 -> FIRST: IsInvoked = FALSE
            //  2.2) f.IsInvoked = false: save 7 8 9 -> NO
            //  3.0) f.IsInvoked = true:  save 1 2 3 -> NO
            //  3.1) f.IsInvoked = true:  save 4 5 6 -> NO
            //  3.2) f.IsInvoked = false: save 7 8 9 -> FIRST: IsInvoked = FALSE
            return thisMethodForEachLevel.First(f => !f.IsInvoked).ActionParsed;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public string View<T>(T model = default(T), string viewName = null, bool searchOnlyInResources = false)
        {
            var view = new RazorView();

            if (viewName != null)
            {
                return view.ProcessByViewName<T>(model, viewName, searchOnlyInResources);
            }
            else
            {
                StackTrace st = new StackTrace();
                StackFrame sf = st.GetFrame(1);
                var executeInfo = new RazorView.ExecuteInfo
                {
                    Method = (MethodInfo)sf.GetMethod(),
                    Type = this.GetType()
                };

                return view.ProcessByViewName<T>(model, executeInfo, searchOnlyInResources);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string ViewContent<T>(T model = default(T), string content = null)
        {
            var view = new RazorView();
            return view.ProcessByContent<T>(model, content);
        }
#endif
        }
}
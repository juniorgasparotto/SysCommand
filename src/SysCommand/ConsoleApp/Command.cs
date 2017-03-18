#if !NETCORE
using SysCommand.ConsoleApp.View.TemplatesGenerator.Razor;
using System.Diagnostics;
#endif

using SysCommand.Execution;
using System.Linq;
using SysCommand.ConsoleApp.View.TemplatesGenerator.T4;
using SysCommand.Parsing;
using System.Runtime.CompilerServices;
using SysCommand.Mapping;
using SysCommand.Helpers;
using System.Reflection;
using System;

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
        
        public ActionMap GetActionMap(Type[] paramTypes, [CallerMemberName] string memberName = "")
        {
            var currentMethod = GetMethod(memberName, paramTypes);
            return GetActionMap(currentMethod);
        }

        public ActionMap GetActionMap(MethodInfo method)
        {
            return this.ExecutionScope.ParseResult.Maps.GetCommandMap(this.GetType()).Methods.FirstOrDefault(m => ReflectionHelper.MethodsAreEquals(m.Method, method));
        }

        public ActionParsed GetAction(Type[] paramTypes, [CallerMemberName] string memberName = "")
        {
            var currentMethod = GetMethod(memberName, paramTypes);
            return GetAction(currentMethod);
        }

        public ActionParsed GetAction(MethodInfo method)
        {
            // return this method (eg: save) in all levels
            // -> save 1 2 3 save 4 5 6 save 7 8 9
            var allMethodResult = this.ExecutionScope.ExecutionResult.Results.With<MethodResult>();
            var thisMethodForEachLevel = allMethodResult.Where(m => ReflectionHelper.MethodsAreEquals(m.ActionParsed.ActionMap.Method, method)).ToList();

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

#if !NETCORE
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ActionMap GetActionMap()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            var currentMethod = (MethodInfo)sf.GetMethod();
            return GetActionMap(currentMethod);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ActionParsed GetAction()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            var currentMethod = (MethodInfo)sf.GetMethod();
            return GetAction(currentMethod);
        }

#endif

        private MethodInfo GetMethod(string memberName, Type[] types)
        {
            var methods = GetType().GetMethods().Where(f => f.Name == memberName);
            MethodInfo currentMethod = null;

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length != types.Length)
                    continue;

                var allEquals = true;
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType != types[i])
                    {
                        allEquals = false;
                        break;
                    }
                }

                if (allEquals)
                {
                    currentMethod = method;
                    break;
                }
            }

            if (currentMethod == null)
                throw new Exception("Method not found");
            return currentMethod;
        }

    }
}

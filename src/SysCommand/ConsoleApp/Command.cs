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
    /// <summary>
    /// The commands represent a grouping of features the same business context, similar to MVC Controllers. 
    /// Programmatically they are represented by classes that inherit from SysCommand.ConsoleApp.Command . 
    /// Each Command instance will have access to the current context by the property this.App .
    /// </summary>
    public abstract class Command : CommandBase
    {
        /// <summary>
        /// App reference
        /// </summary>
        public App App { get; internal set; }

        /// <summary>
        /// Get the argument parsed (property) by name
        /// </summary>
        /// <param name="name">Property name</param>
        /// <returns>Instance of ArgumentParsed</returns>
        public ArgumentParsed GetArgument(string name)
        {
            // return all properties results for this instance
            var allPropertiesResult = this.ExecutionScope.ExecutionResult.Results.With<PropertyResult>();
            var thisMethodForEachLevel = allPropertiesResult.Where(m => m.Target == this).ToList();

            return thisMethodForEachLevel.LastOrDefault(f => f.Name == name)?.ArgumentParsed;
        }

        /// <summary>
        /// Get a view result by T4 template
        /// </summary>
        /// <typeparam name="TTemplate">Type of template</typeparam>
        /// <returns>Text for view</returns>
        public string ViewT4<TTemplate>()
        {
            return T4Helper.Execute<TTemplate, object>(null);
        }

        /// <summary>
        /// Get a view result by T4 template
        /// </summary>
        /// <typeparam name="TTemplate">Type of template</typeparam>
        /// <typeparam name="TModel">Type of model</typeparam>
        /// <param name="model">Instance of model</param>
        /// <returns>Text for view</returns>
        public string ViewT4<TTemplate, TModel>(TModel model = default(TModel))
        {
            return T4Helper.Execute<TTemplate, TModel>(model);
        }

        /// <summary>
        /// Get the ActionMap by parameters type
        /// </summary>
        /// <param name="paramTypes">List of parameter type</param>
        /// <param name="memberName">Method name</param>
        /// <returns>Instance of ActionMap</returns>
        public ActionMap GetActionMap(Type[] paramTypes, [CallerMemberName] string memberName = "")
        {
            var currentMethod = GetMethod(memberName, paramTypes);
            return GetActionMap(currentMethod);
        }

        /// <summary>
        ///  Get the ActionMap by MethodInfo
        /// </summary>
        /// <param name="method">MethodInfo reference</param>
        /// <returns>Instance of ActionMap</returns>
        public ActionMap GetActionMap(MethodInfo method)
        {
            return this.ExecutionScope.ParseResult.Maps.GetCommandMap(this.GetType()).Methods.FirstOrDefault(m => ReflectionHelper.MethodsAreEquals(m.Method, method));
        }

        /// <summary>
        /// Get the ActionParsed by parameters type
        /// </summary>
        /// <param name="paramTypes">List of parameter type</param>
        /// <param name="memberName">Method name</param>
        /// <returns>Instance of ActionParsed</returns>
        public ActionParsed GetAction(Type[] paramTypes, [CallerMemberName] string memberName = "")
        {
            var currentMethod = GetMethod(memberName, paramTypes);
            return GetAction(currentMethod);
        }

        /// <summary>
        /// Get the ActionParsed by MethodInfo
        /// </summary>
        /// <param name="method">MethodInfo reference</param>
        /// <returns>Instance of ActionParsed</returns>
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
        /// <summary>
        /// Get a view result by RazorEngine from the current method
        /// </summary>
        /// <typeparam name="T">Type of model</typeparam>
        /// <param name="model">Model object</param>
        /// <param name="viewName">View name. If null get the method name as reference</param>
        /// <param name="searchOnlyInResources">If false, the search will occur in the file structure and in the "Embedded Resource".</param>
        /// <returns>Text for view</returns>
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

        /// <summary>
        /// Get a view result by RazorEngine from the content
        /// </summary>
        /// <typeparam name="T">Type of model</typeparam>
        /// <param name="model">Model object</param>
        /// <param name="content">Razor content</param>
        /// <returns>Text for view</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public string ViewContent<T>(T model = default(T), string content = null)
        {
            var view = new RazorView();
            return view.ProcessByContent<T>(model, content);
        }

        /// <summary>
        /// Get ActionMap from the current method
        /// </summary>
        /// <returns>Instance of ActionMap</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public ActionMap GetActionMap()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);
            var currentMethod = (MethodInfo)sf.GetMethod();
            return GetActionMap(currentMethod);
        }

        /// <summary>
        /// Get ActionParsed from the current method
        /// </summary>
        /// <returns>Instance of ActionParsed</returns>
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

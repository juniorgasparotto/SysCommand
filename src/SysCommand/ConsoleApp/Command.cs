//#if !NETCORE1_6
using SysCommand.ConsoleApp.View.TemplatesGenerator.Razor;
using System.Diagnostics;
//#endif

using SysCommand.Execution;
using System.Linq;
using SysCommand.ConsoleApp.View.TemplatesGenerator.T4;
using SysCommand.Parsing;
using System.Runtime.CompilerServices;
using SysCommand.Mapping;
using SysCommand.Helpers;
using System.Reflection;
using System;
using SysCommand.ConsoleApp.Files;
using Newtonsoft.Json;
using SysCommand.ConsoleApp.View;
using System.Collections.Generic;
using System.Threading.Tasks;

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


        #region View - Razor
        
        /// <summary>
        /// Parse a razor view from the current method or "viewName" with model
        /// </summary>
        /// <typeparam name="T">Type of model</typeparam>
        /// <param name="model">Model object</param>
        /// <param name="viewName">View name. If null get the method name as reference</param>
        /// <param name="searchOnlyInResources">If false, the search will occur in the file structure and in the "Embedded Resource".</param>
        /// <returns>Text for view</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual async Task<string> ViewAsync<T>(T model = default(T), string viewName = null, bool searchOnlyInResources = false, [CallerMemberName] string methodCaller = "")
        {
            return await ViewAsyncInternal(true, model, methodCaller, viewName, searchOnlyInResources);
        }

        /// <summary>
        /// Parse a razor view from the current method or "viewName" without Model
        /// </summary>
        /// <param name="viewName">View name. If null get the method name as reference</param>
        /// <param name="searchOnlyInResources">If false, the search will occur in the file structure and in the "Embedded Resource".</param>
        /// <returns>Text for view</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual async Task<string> ViewAsync(string viewName = null, bool searchOnlyInResources = false, [CallerMemberName] string methodCaller = "")
        {
            return await ViewAsyncInternal(false, default(object), methodCaller, viewName, searchOnlyInResources);
        }

        /// <summary>
        /// Parse a razor view from the current method or "viewName" with model
        /// </summary>
        /// <typeparam name="T">Type of model</typeparam>
        /// <param name="model">Model object</param>
        /// <param name="viewName">View name. If null get the method name as reference</param>
        /// <param name="searchOnlyInResources">If false, the search will occur in the file structure and in the "Embedded Resource".</param>
        /// <returns>Text for view</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual string View<T>(T model = default(T), string viewName = null, bool searchOnlyInResources = false, [CallerMemberName] string methodCaller = "")
        {
            return ViewAsyncInternal(true, model, methodCaller, viewName, searchOnlyInResources).Result;
        }

        /// <summary>
        /// Parse a razor view from the current method without model
        /// </summary>
        /// <param name="viewName">View name. If null get the method name as reference</param>
        /// <param name="searchOnlyInResources">If false, the search will occur in the file structure and in the "Embedded Resource".</param>
        /// <returns>Text for view</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual string View(string viewName = null, bool searchOnlyInResources = false, [CallerMemberName] string methodCaller = "")
        {
            return ViewAsyncInternal(false, default(object), methodCaller, viewName, searchOnlyInResources).Result;
        }

        private async Task<string> ViewAsyncInternal<T>(bool useModel, T model = default(T), string methodCaller = null, string viewName = null, bool searchOnlyInResources = false)
        {
            var template = new RazorTemplateHelper();

            if (viewName != null)
            {
                return await template.ProcessByViewNameAsync<T>(model, viewName, useModel, searchOnlyInResources);
            }
            else
            {
                var executeInfo = new RazorTemplateHelper.ExecuteInfo
                {
                    Method = methodCaller,
                    Type = this.GetType()
                };

                return await template.ProcessByViewNameAsync<T>(model, executeInfo, useModel, searchOnlyInResources);
            }
        }

        /// <summary>
        /// Parse a razor view from the content string with model
        /// </summary>
        /// <typeparam name = "T" > Type of model</typeparam>
        /// <param name = "content" > Razor content</param>
        /// <param name = "model" > Model object</param>
        /// <returns>Text for view</returns>
        public virtual async Task<string> ViewContentAsync<T>(string content, T model = default(T))
        {
            var template = new RazorTemplateHelper();
            return await template.ProcessByContentAsync(content, model, true);
        }

        /// <summary>
        /// Parse a razor view from the content string without model
        /// </summary>
        /// <param name = "content" > Razor content</param>
        /// <param name = "model" > Model object</param>
        /// <returns>Text for view</returns>
        public virtual async Task<string> ViewContentAsync(string content)
        {
            var template = new RazorTemplateHelper();
            return await template.ProcessByContentAsync(content, default(object), false);
        }

        /// <summary>
        /// Parse a razor view from the content string with model
        /// </summary>
        /// <typeparam name = "T" > Type of model</typeparam>
        /// <param name = "content" > Razor content</param>
        /// <param name = "model" > Model object</param>
        /// <returns>Text for view</returns>
        public virtual string ViewContent<T>(string content, T model = default(T))
        {
            return ViewContentAsync(content, model).Result;
        }

        /// <summary>
        /// Parse a razor view from the content string without model
        /// </summary>
        /// <param name = "content" > Razor content</param>
        /// <param name = "model" > Model object</param>
        /// <returns>Text for view</returns>
        public virtual string ViewContent(string content)
        {
            return ViewContentAsync(content).Result;
        }

        #endregion

        #region Views - T4/Json/Table
        
        /// <summary>
        /// Get a view result by T4 template
        /// </summary>
        /// <typeparam name="TTemplate">Type of template</typeparam>
        /// <returns>Text for view</returns>
        public virtual string ViewT4<TTemplate>()
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
        public virtual string ViewT4<TTemplate, TModel>(TModel model = default(TModel))
        {
            return T4Helper.Execute<TTemplate, TModel>(model);
        }

        /// <summary>
        /// View model as Json
        /// </summary>
        /// <param name="model">Model instance</param>
        /// <param name="config">JsonSerializerSettings instance</param>
        /// <returns>Object to JSON string</returns>
        public virtual string ViewJson(object model, JsonSerializerSettings config = null)
        {
            return JsonFileManager.GetContentJsonFromObject(model, config);
        }

        /// <summary>
        /// View list of object like table
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="list">List of T</param>
        /// <param name="colWidth">Column width</param>
        /// <param name="config">Config callback</param>
        /// <returns></returns>
        public virtual string ViewTable<T>(IEnumerable<T> list, int colWidth = 0, Action<TableView> config = null)
        {
            var tableView = TableView.ToTableView(list, colWidth)
                        .Build();
            config?.Invoke(tableView);
            return tableView.ToString();
        }

        #endregion

        #region Get current arguments/actions at runtime

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

        #endregion
    }
}

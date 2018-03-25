using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal static class AssemblyDiscovery
    {
        private static List<Assembly> list = null;

        public static void LoadAssembly<TInternalType>()
        {
            // this method is used only to load de assembly
            list = null;
        }

        public static IEnumerable<Assembly> GetAllLoaded(bool refresh = false)
        {
            if (list == null || refresh)
            {
                #region taghelper - not implemmented

                // Load Assembly Microsoft.AspNetCore.Razor.Runtime.dll
                //LoadAssembly<Microsoft.AspNetCore.Razor.TagHelpers.TagMode>();

                //// load assembly to use attributes in generated class
                //// Microsoft.AspNetCore.Mvc.Razor.dll
                //LoadAssembly<Microsoft.AspNetCore.Mvc.Razor.Compilation.IViewCompiler>();

                //// Microsoft.AspNetCore.Razor.Runtime.dll
                //LoadAssembly<Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner>();
                //LoadAssembly<Microsoft.AspNetCore.Razor.TagHelpers.HtmlTargetElementAttribute>();
                //LoadAssembly<Microsoft.AspNetCore.Razor.Language.DirectiveUsage>();

                #endregion

                // support dotnet45+/dotnetcore2.0
                // enable 'dynamic' type
                // Microsoft.CSharp.dll
                LoadAssembly<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>();

                // support dotnet45+
                // Microsoft.AspNetCore.Razor.Runtime.dll
                LoadAssembly<Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItem>();

                // enable 'dynamic' type
                // netstandard.dll
                //LoadAssembly<System.Runtime.CompilerServices.DynamicAttribute>();

                // AssemblyTemplateAttribute: not load template assemblies
                list = new List<Assembly>();
                list.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic
                    && p.GetCustomAttribute<AssemblyTemplateAttribute>() == null));
            }

            return list;
        }
    }
}
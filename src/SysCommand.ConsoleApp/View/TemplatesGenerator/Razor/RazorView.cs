#if !(NETSTANDARD1_6)
using SysCommand.ConsoleApp.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    public class RazorView
    {
        public static string FileExtension = ".razor";
        private RazorTemplateGenerator generator;

        public class ExecuteInfo
        {
            public Type Type { get; set; }
            public MethodInfo Method { get; set; }
        }

        public RazorView()
        {
            generator = new RazorTemplateGenerator();
        }

        public string ProcessByViewName<T>(T model, ExecuteInfo info, bool searchOnlyInResources = false)
        {
            string content = null;

            if (searchOnlyInResources)
            {
                content = this.FindContentInResourse(info);
            }
            else
            {
                content = this.FindContentInFiles(info, false);
                if (content == null)
                    content = this.FindContentInResourse(info);
            }

            return this.ProcessByContent<T>(model, content);
        }

        public string ProcessByViewName<T>(T model, string viewName, bool searchOnlyInResources = false)
        {
            string content = null;

            if (searchOnlyInResources)
            {
                content = this.FindContentInResourse(viewName);
            }
            else
            {
                content = this.FindContentInFiles(viewName, false);
                if (content == null)
                    content = this.FindContentInResourse(viewName);
            }

            return this.ProcessByContent<T>(model, content);
        }

        public string ProcessByContent<T>(T model, string content)
        {
            generator.RegisterTemplate<T>(content);
            generator.CompileTemplates();
            return generator.GenerateOutput<T>(model);
        }

        public string GetMethodName(MethodInfo method)
        {
            return method.Name + FileExtension;
        }

        public string GetTypeName(Type type)
        {
            var name = type.Name;
            Regex rgx = new Regex("Command$", RegexOptions.IgnoreCase);
            string result = rgx.Replace(name, "");
            return result;
        }

        private string FindContentInFiles(ExecuteInfo info, bool throwException = true)
        {
            string root;
            //if (IsDebug)
            //    root = "../../Views/";
            //else
            root = "Views/";

            var command = this.GetTypeName(info.Type) + "/";
            var method = this.GetMethodName(info.Method);
            string fileNameLevel0 = method;
            string fileNameLevel1 = root + method;
            string fileNameLevel2 = root + command + method;
            if (FileHelper.FileExists(fileNameLevel2))
                return FileHelper.GetContentFromFile(fileNameLevel2);
            else if (FileHelper.FileExists(fileNameLevel1))
                return FileHelper.GetContentFromFile(fileNameLevel1);
            else if (FileHelper.FileExists(fileNameLevel0))
                return FileHelper.GetContentFromFile(fileNameLevel0);

            if (throwException)
                this.ThrowFindContentInFiles();

            return null;
        }

        private string FindContentInFiles(string viewName, bool throwException = true)
        {
            if (FileHelper.FileExists(viewName))
                return FileHelper.GetContentFromFile(viewName);

            if (throwException)
                this.ThrowFindContentInFiles();

            return null;
        }

        private void ThrowFindContentInFiles()
        {
            throw new Exception(string.Format("No view was found in the files struct."));
        }

        public string FindContentInResourse(ExecuteInfo info, bool throwException = true)
        {
            var viewName = this.GetTypeName(info.Type) + "." + this.GetMethodName(info.Method);
            var content = FindContentInResourse(viewName, false);

            if (content == null)
            {
                viewName = this.GetMethodName(info.Method);
                content = FindContentInResourse(viewName, false);
            }

            if (content == null && throwException)
                ThrowFindContentInResourse(viewName, GetResourceAssembly().FullName);

            return content;
        }

        public string FindContentInResourse(string viewName, bool throwException = true)
        {
            var curAssembly = GetResourceAssembly();
            var name = curAssembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.ToLower().EndsWith(viewName.ToLower()));

            if (name != null)
            {
                using (var reader = new StreamReader(curAssembly.GetManifestResourceStream(name)))
                {
                    return reader.ReadToEnd();
                }
            }

            if (throwException)
                ThrowFindContentInResourse(viewName, curAssembly.FullName);

            return null;
        }

        private void ThrowFindContentInResourse(string actionName, string assemblyName)
        {
            throw new Exception(string.Format("No view, for the action '{0}', was found in the assembly resources '{1}'.", actionName, assemblyName));
        }

        private Assembly GetResourceAssembly()
        {
            return Assembly.GetEntryAssembly();
        }
    }
}
#endif
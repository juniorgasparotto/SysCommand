using SysCommand.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SysCommand.ConsoleApp
{
    public class View
    {
        private bool IsDebug { get { return System.Diagnostics.Debugger.IsAttached; } }

        public class ExecuteInfo
        {
            public Type Type { get; set; }
            public MethodInfo Method { get; set; }
        }

        private RazorTemplateGenerator generator;

        public View()
        {
            generator = new RazorTemplateGenerator();
        }

        public string ProcessByViewName<T>(T model, ExecuteInfo info, bool searchInResourse = false)
        {
            var type = model.GetType();
            string content = null;

            if (searchInResourse)
                content = this.FindContentInResourse(info);
            else
                content = this.FindContentInFiles(info);

            return this.ProcessByContent<T>(model, content);
        }

        private string FindContentInFiles(ExecuteInfo info)
        {
            string root;
            if (IsDebug)
                root = "../../Views/";
            else
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

            throw new Exception(string.Format("No view was found in the files struct."));
        }

        public string ProcessByContent<T>(T model, string content)
        {
            generator.RegisterTemplate<T>(content);
            generator.CompileTemplates();
            return generator.GenerateOutput<T>(model);
        }

        public string GetMethodName(MethodInfo method)
        {
            return method.Name + ".view";
        }

        public string GetTypeName(Type type)
        {
            var name = type.Name;
            Regex rgx = new Regex("Command$", RegexOptions.IgnoreCase);
            string result = rgx.Replace(name, "");
            return result;
        }

        public string FindContentInResourse(ExecuteInfo info)
        {
            var viewName = this.GetTypeName(info.Type) + "." + this.GetMethodName(info.Method);
            var curAssembly = Assembly.GetExecutingAssembly();
            var name = curAssembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.ToLower().EndsWith(viewName.ToLower()));

            if (name != null)
            {
                using (var reader = new StreamReader(curAssembly.GetManifestResourceStream(name)))
                {
                    return reader.ReadToEnd();
                }
            }

            throw new Exception(string.Format("No view was found in the assembly resources '{0}'.", curAssembly.FullName));
        }
    }
}

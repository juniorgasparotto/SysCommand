using SysCommand.ConsoleApp.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal class RazorTemplateHelper
    {
        public static string FileExtension { get; set; } = ".cshtml";
        private RazorTemplate template;

        public class ExecuteInfo
        {
            public Type Type { get; set; }
            public string Method { get; set; }
        }

        public RazorTemplateHelper()
        {
            template = new RazorTemplate();
        }

        public async Task<string> ProcessByViewNameAsync<T>(T model, ExecuteInfo info, bool useModel, bool searchOnlyInResources = false)
        {
            if (searchOnlyInResources)
            {
                var content = this.FindContentInResourse(info);
                if (useModel)
                    return await template.ParseContentAsync(content, model);
                else
                    return await template.ParseContentAsync(content);
            }
            else
            {
                var fileName = this.FindFileNameInFiles(info, false);
                if (fileName == null)
                {
                    var content = this.FindContentInResourse(info, false);
                    if (content == null)
                        this.ThrowFindContentInFiles();

                    if (useModel)
                        return await template.ParseContentAsync(content, model);
                    else
                        return await template.ParseContentAsync(content);
                }
                else
                {
                    if (useModel)
                        return await template.ParseAsync(fileName, model);
                    else
                        return await template.ParseAsync(fileName);
                }
            }
        }

        public async Task<string> ProcessByViewNameAsync<T>(T model, string viewName, bool useModel, bool searchOnlyInResources = false)
        {
            if (searchOnlyInResources)
            {
                var content = this.FindContentInResourse(viewName);
                if (useModel)
                    return await template.ParseContentAsync(content, model);
                else
                    return await template.ParseContentAsync(content);
            }
            else
            {
                var fileName = this.FindFileNameInFiles(viewName, false);
                if (fileName == null)
                {
                    var content = this.FindContentInResourse(viewName, false);
                    if (content == null)
                        this.ThrowFindContentInFiles();

                    if (useModel)
                        return await template.ParseContentAsync(content, model);
                    else
                        return await template.ParseContentAsync(content);
                }
                else
                {
                    if (useModel)
                        return await template.ParseAsync(fileName, model);
                    else
                        return await template.ParseAsync(fileName);
                }
            }
        }

        public async Task<string> ProcessByContentAsync<T>(string content, T model, bool useModel)
        {
            if (useModel)
                return await template.ParseContentAsync(content, model);
            else
                return await template.ParseContentAsync(content);
        }

        public string GetMethodName(string method)
        {
            return method + FileExtension;
        }

        public string GetTypeName(Type type)
        {
            var name = type.Name;
            Regex rgx = new Regex("Command$", RegexOptions.IgnoreCase);
            string result = rgx.Replace(name, "");
            return result;
        }

        private string FindFileNameInFiles(ExecuteInfo info, bool throwException = true)
        {
            var root = "Views";

            if (Development.IsAttached)
                root = Path.Combine(Development.GetProjectDirectory(), root);

            var command = this.GetTypeName(info.Type) + "/";
            var method = this.GetMethodName(info.Method);
            string fileNameLevel0 = method;
            string fileNameLevel1 = root + "/" + method;
            string fileNameLevel2 = root + "/" + command + method;
            if (FileHelper.FileExists(fileNameLevel2))
                return NormalizePath(fileNameLevel2);
            else if (FileHelper.FileExists(fileNameLevel1))
                return NormalizePath(fileNameLevel1);
            else if (FileHelper.FileExists(fileNameLevel0))
                return NormalizePath(fileNameLevel0);

            if (throwException)
                this.ThrowFindContentInFiles();

            return null;
        }

        private string FindFileNameInFiles(string viewName, bool throwException = true)
        {
            if (Development.IsAttached)
                viewName = Path.Combine(Development.GetProjectDirectory(), viewName);

            if (FileHelper.FileExists(viewName))
                return NormalizePath(viewName);

            if (throwException)
                this.ThrowFindContentInFiles();

            return null;
        }

        private string NormalizePath(string path)
        {
            return path.Replace("/", "\\");
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
                ThrowFindContentInResourse(viewName);

            return content;
        }

        public string FindContentInResourse(string viewName, bool throwException = true)
        {
            var assembliesCandidates = AssemblyDiscovery.GetAllLoaded().Where(f => !f.FullName.StartsWith("System") && !f.FullName.StartsWith("Microsoft"));

            foreach (var curAssembly in assembliesCandidates)
            {
                var name = curAssembly
                    .GetManifestResourceNames()
                    .FirstOrDefault(n => n.ToLower().EndsWith(viewName.Replace("/", ".").ToLower()));

                if (name != null)
                {
                    using (var reader = new StreamReader(curAssembly.GetManifestResourceStream(name)))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            if (throwException)
                ThrowFindContentInResourse(viewName);

            return null;
        }

        private void ThrowFindContentInResourse(string actionName)
        {
            throw new Exception(string.Format("No view, for the action '{0}', was found in the assemblies.", actionName));
        }
    }
}
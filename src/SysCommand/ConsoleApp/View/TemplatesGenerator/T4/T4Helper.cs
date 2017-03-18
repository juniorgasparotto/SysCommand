using SysCommand.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.T4
{
    public static class T4Helper
    {
        public static string Execute<TTemplate, TModel>(TModel model = default(TModel))
        {
            var genAttribute = typeof(TTemplate).GetCustomAttribute<global::System.CodeDom.Compiler.GeneratedCodeAttribute>(true);
            if (genAttribute != null)
            {
                var template = Activator.CreateInstance<TTemplate>();
                var session = template
                    .GetType()
                    .GetProperties()
                    .FirstOrDefault(f => f.Name == "Session");
                var initialize = template
                    .GetType()
                    .GetMethods()
                    .FirstOrDefault(f => f.Name == "Initialize");
                var transform = template
                    .GetType()
                    .GetMethods()
                    .FirstOrDefault(f => f.Name == "TransformText");

                if (session != null && initialize != null && transform != null)
                {
                    var dic = new Dictionary<string, object>();
                    dic.Add("Model", model);
                    session.SetValue(template, dic);
                    initialize.Invoke(template, null);
                    var output = transform.Invoke(template, null);
                    return output.ToString();
                }
            }

            throw new Exception(string.Format("The type '{0}' is not a T4 file.", typeof(TTemplate).Name));
        }
    }
}

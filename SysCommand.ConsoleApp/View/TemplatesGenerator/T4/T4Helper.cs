using SysCommand.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SysCommand.ConsoleApp
{
    public static class T4Helper
    {
        public static string Execute<TTemplate, TModel>(TModel model = default(TModel))
        {
            var genAttributeType = typeof(global::System.CodeDom.Compiler.GeneratedCodeAttribute);
            var genAttribute = typeof(TTemplate).GetCustomAttributes(true)
                .FirstOrDefault() as global::System.CodeDom.Compiler.GeneratedCodeAttribute;
            if (genAttribute != null)
            {
                var template = Activator.CreateInstance<TTemplate>();
                var session = template
                    .GetType()
                    .GetProperties()
                    .Where(f => f.Name == "Session")
                    .FirstOrDefault();
                var initialize = template
                    .GetType()
                    .GetMethods()
                    .Where(f => f.Name == "Initialize")
                    .FirstOrDefault();
                var transform = template
                    .GetType()
                    .GetMethods()
                    .Where(f => f.Name == "TransformText")
                    .FirstOrDefault();

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

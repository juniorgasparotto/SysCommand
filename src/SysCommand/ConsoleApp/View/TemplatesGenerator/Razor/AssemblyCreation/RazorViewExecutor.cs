using SysCommand.Helpers;
using System;
using System.Threading.Tasks;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal class RazorViewExecutor
    {
        public async Task<string> ExecuteAsync<T>(Type compiledType, T model)
        {
            string output = null;

            if (model != null && ReflectionHelper.IsAnonymousType(typeof(T)))
            {
                var result = new RouteValueDictionary(model);
                var view = (RazorView<dynamic>)Activator.CreateInstance(compiledType);

                if (result.Count != 0)
                    view.Model = new DynamicData(result);

                await view.ExecuteAsync();
                output = view.ToString();
            }
            else
            {
                var view = (RazorView<T>)Activator.CreateInstance(compiledType);
                view.Model = model;
                await view.ExecuteAsync();
                output = view.ToString();
            }

            return output;
        }
    }
}
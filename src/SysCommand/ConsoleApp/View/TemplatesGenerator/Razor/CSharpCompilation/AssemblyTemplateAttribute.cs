using System;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyTemplateAttribute : Attribute
    {
        public AssemblyTemplateAttribute(string key, Type templateType)
        {
            Key = key;
            TemplateType = templateType;
        }

        /// <summary>
        /// Gets the key of the view.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the template type.
        /// </summary>
        public Type TemplateType { get; }
    }
}
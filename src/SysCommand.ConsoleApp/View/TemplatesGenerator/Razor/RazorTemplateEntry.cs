using System;

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    public class RazorTemplateEntry
    {
        public Type ModelType { get; set; }
        public string TemplateString { get; set; }
        public string TemplateName { get; set; }
    }
}

namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    internal class RazorCompilerResult
    {
        public string GeneratedText { get; set; }
        public RazorCompilerException ProcessingException { get; set; }
    }
}
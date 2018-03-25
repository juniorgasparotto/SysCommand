namespace SysCommand.ConsoleApp.View.TemplatesGenerator.Razor
{
    public abstract class RazorView<T> : RazorViewBase
    {
        public T Model { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
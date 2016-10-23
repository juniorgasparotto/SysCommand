namespace SysCommand.Evaluation
{
    public interface IMemberResult
    {
        string Name { get; }
        object Source { get; }
        object Value { get; set; }
        bool IsInvoked { get; set; }
        void Invoke();
    }
}

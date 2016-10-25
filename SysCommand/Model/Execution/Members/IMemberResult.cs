namespace SysCommand.Execution
{
    public interface IMemberResult
    {
        string Name { get; }
        object Target { get; }
        object Value { get; set; }
        bool IsInvoked { get; set; }
        void Invoke();
    }
}

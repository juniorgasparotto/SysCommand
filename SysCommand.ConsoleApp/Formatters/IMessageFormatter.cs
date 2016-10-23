using SysCommand.Parsing;

namespace SysCommand.ConsoleApp
{
    public interface IMessageFormatter
    {
        void ShowErrors(ApplicationResult appResult);
        void ShowNotFound(ApplicationResult appResult);
        void ShowMethodReturn(ApplicationResult appResult, IMember method, object value);
        string GetMethodSpecification(ActionMap map);
        string GetPropertyErrorDescription(ArgumentMapped argumentMapped);
    }
}
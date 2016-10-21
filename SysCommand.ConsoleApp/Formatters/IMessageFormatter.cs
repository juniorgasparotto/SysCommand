using SysCommand.Parser;

namespace SysCommand.ConsoleApp
{
    public interface IMessageFormatter
    {
        string GetMethodSpecification(ActionMap map);
        string GetPropertyErrorDescription(ArgumentMapped argumentMapped);
        void ShowErrors(ApplicationResult appResult);
        void ShowNotFound(ApplicationResult appResult);
    }
}
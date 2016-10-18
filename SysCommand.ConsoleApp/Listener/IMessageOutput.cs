using SysCommand.Parser;

namespace SysCommand.ConsoleApp
{
    public interface IMessageOutput
    {
        string GetMethodSpecification(ActionMap map);
        string GetPropertyErrorDescription(ArgumentMapped argumentMapped);
        void ShowErrors(AppResult appResult);
        void ShowNotFound(AppResult appResult);
    }
}
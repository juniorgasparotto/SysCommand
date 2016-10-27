using SysCommand.Execution;
using SysCommand.Mapping;
using SysCommand.Parsing;

namespace SysCommand.ConsoleApp
{
    public interface IDescriptor
    {
        void ShowErrors(ApplicationResult appResult);
        void ShowNotFound(ApplicationResult appResult);
        void ShowMethodReturn(ApplicationResult appResult, IMemberResult method, object value);
        string GetMethodSpecification(ActionMap map);
        string GetPropertyErrorDescription(ArgumentParsed argumentParsed);
    }
}
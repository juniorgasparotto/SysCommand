using System.Collections.Generic;
using SysCommand.Execution;
using SysCommand.Mapping;

namespace SysCommand.ConsoleApp.Descriptor
{
    public interface IDescriptor
    {
        void ShowErrors(ApplicationResult appResult);
        void ShowNotFound(ApplicationResult appResult);
        void ShowMethodReturn(ApplicationResult appResult, IMemberResult method, object value);
        string GetHelpText(IEnumerable<CommandMap> commandMaps);
        string GetHelpText(IEnumerable<CommandMap> commandMaps, string actionName);
    }
}
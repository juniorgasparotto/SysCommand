using System.Collections.Generic;
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
        string GetHelpText(IEnumerable<CommandMap> commandMaps);
        //string GetHelpText(CommandMap commandMap);
        //string GetHelpText(IEnumerable<ActionMap> actionsMap);
        //string GetHelpText(ActionMap actionMap, int padding, int paddingParams);
        //string GetHelpText(IEnumerable<ArgumentMap> argumentMap, int padding);
        //string GetHelpText(ArgumentMap argumentMap);
        //string GetArgumentNameWithPrefix(ArgumentMap arg);
    }
}
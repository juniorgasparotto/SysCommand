using System.Collections.Generic;
using SysCommand.Execution;
using SysCommand.Mapping;

namespace SysCommand.ConsoleApp.Descriptor
{
    /// <summary>
    /// Exposes methods for displaying the texts to the end user.
    /// </summary>
    public interface IDescriptor
    {
        /// <summary>
        /// Format and display all errors
        /// </summary>
        /// <param name="appResult">Result base</param>
        void ShowErrors(ApplicationResult appResult);

        /// <summary>
        /// Format and display a not found message
        /// </summary>
        /// <param name="appResult">Result base</param>
        void ShowNotFound(ApplicationResult appResult);

        /// <summary>
        /// Display a value for a return method
        /// </summary>
        /// <param name="appResult">Return base</param>
        /// <param name="method">MethodResult reference</param>
        /// <param name="value">Value to display</param>
        void ShowMethodReturn(ApplicationResult appResult, IMemberResult method, object value);

        /// <summary>
        /// Get a formatted help text
        /// </summary>
        /// <param name="commandMaps">List of commands maps</param>
        /// <returns>Help text</returns>
        string GetHelpText(IEnumerable<CommandMap> commandMaps);

        /// <summary>
        /// Get a formatted help text to a specific action
        /// </summary>
        /// <param name="commandMaps">List of commands maps</param>
        /// <param name="actionName">Action name</param>
        /// <returns>Help text</returns>
        string GetHelpText(IEnumerable<CommandMap> commandMaps, string actionName);
    }
}
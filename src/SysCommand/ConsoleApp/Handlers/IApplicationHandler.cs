using SysCommand.Execution;
using System;

namespace SysCommand.ConsoleApp.Handlers
{
    public interface IApplicationHandler
    {
        /// <summary>
        /// Fires at the end of the implementation
        /// </summary>
        /// <param name="appResult">Result base</param>
        void OnComplete(ApplicationResult eventArgs);

        /// <summary>
        /// Fires in case of exception.
        /// </summary>
        /// <param name="appResult">Result base</param>
        /// <param name="ex">Last unhandled exception</param>
        void OnException(ApplicationResult eventArgs, Exception ex);

        /// <summary>
        /// Fires before invoking each Member (property or method) that was parsed.
        /// </summary>
        /// <param name="appResult">Result base</param>
        /// <param name="member">Current member in execution</param>
        void OnBeforeMemberInvoke(ApplicationResult eventArgs, IMemberResult member);

        /// <summary>
        /// Fires after invoking each Member (property or method) that was parsed.
        /// </summary>
        /// <param name="appResult">Result base</param>
        /// <param name="member">Current member in execution</param>
        void OnAfterMemberInvoke(ApplicationResult eventArgs, IMemberResult member);

        /// <summary>
        /// Fires when a method returns value
        /// </summary>
        /// <param name="appResult">Result base</param>
        /// <param name="method">Current method in execution</param>
        void OnMethodReturn(ApplicationResult eventArgs, IMemberResult method);
    }
}
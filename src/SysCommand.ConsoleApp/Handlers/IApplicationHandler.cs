using SysCommand.Execution;
using System;

namespace SysCommand.ConsoleApp.Handlers
{
    public interface IApplicationHandler
    {
        void OnComplete(ApplicationResult eventArgs);
        void OnException(ApplicationResult eventArgs, Exception ex);
        void OnBeforeMemberInvoke(ApplicationResult eventArgs, IMemberResult member);
        void OnAfterMemberInvoke(ApplicationResult eventArgs, IMemberResult member);
        void OnMethodReturn(ApplicationResult eventArgs, IMemberResult method);
    }
}
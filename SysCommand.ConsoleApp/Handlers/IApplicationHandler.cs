using System;

namespace SysCommand.ConsoleApp
{
    public interface IApplicationHandler
    {
        void OnComplete(ApplicationResult eventArgs);
        void OnException(ApplicationResult eventArgs, Exception ex);
        void OnBeforeMemberInvoke(ApplicationResult eventArgs, IMember member);
        void OnAfterMemberInvoke(ApplicationResult eventArgs, IMember member);
        void OnMethodReturn(ApplicationResult eventArgs, IMember method);
    }
}
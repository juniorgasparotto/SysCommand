using System;

namespace SysCommand.ConsoleApp
{
    public interface IEventListener
    {
        void OnComplete(AppResult eventArgs);
        void OnException(AppResult eventArgs, Exception ex);
        void OnBeforeMemberInvoke(AppResult eventArgs, IMember member);
        void OnAfterMemberInvoke(AppResult eventArgs, IMember member);
        void OnMethodReturn(AppResult eventArgs, IMember method);
    }
}
using System;
using System.Collections.Generic;

namespace SysCommand.ConsoleApp
{
    public interface IEventListener
    {
        void OnComplete(AppResult eventArgs);
        void OnException(AppResult eventArgs, Exception ex);
        void OnBeforeMemberInvoke(AppResult eventArgs, IMember member);
        void OnAfterMemberInvoke(AppResult eventArgs, IMember member);
        void OnMemberPrint(AppResult eventArgs, IMember method);
        //void OnSuccess(string[] args, IEnumerable<CommandMap> map, Result<IMember> result);
        //void OnNotFound(string[] args, IEnumerable<CommandMap> map, Result<IMember> result);
        //void OnInvalidArgumentParse(string[] args, IEnumerable<CommandMap> map, Result<IMember> result);
        //void OnError(string[] args, IEnumerable<CommandMap> map, Result<IMember> parseResult, Exception ex);
    }
}
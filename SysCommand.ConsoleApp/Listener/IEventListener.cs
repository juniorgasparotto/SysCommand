using System;
using System.Collections.Generic;

namespace SysCommand.ConsoleApp
{
    public interface IEventListener
    {
        void OnComplete(AppEventsArgs eventArgs);
        void OnException(AppEventsArgs eventArgs, Exception ex);
        void OnBeforeMemberInvoke(AppEventsArgs eventArgs, IMember member);
        void OnAfterMemberInvoke(AppEventsArgs eventArgs, IMember member);
        void OnPrint(AppEventsArgs eventArgs, IMember method);
        //void OnSuccess(string[] args, IEnumerable<CommandMap> map, Result<IMember> result);
        //void OnNotFound(string[] args, IEnumerable<CommandMap> map, Result<IMember> result);
        //void OnInvalidArgumentParse(string[] args, IEnumerable<CommandMap> map, Result<IMember> result);
        //void OnError(string[] args, IEnumerable<CommandMap> map, Result<IMember> parseResult, Exception ex);
    }
}
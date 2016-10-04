using System;
using System.Collections.Generic;

namespace SysCommand.ConsoleApp
{
    public interface IEventListener
    {
        void OnComplete(App app, EvalState state);
        void OnException(App app, Exception ex);
        //void OnSuccess(string[] args, IEnumerable<CommandMap> map, Result<IMember> result);
        //void OnNotFound(string[] args, IEnumerable<CommandMap> map, Result<IMember> result);
        //void OnInvalidArgumentParse(string[] args, IEnumerable<CommandMap> map, Result<IMember> result);
        //void OnError(string[] args, IEnumerable<CommandMap> map, Result<IMember> parseResult, Exception ex);
    }
}
using System;
using System.Collections.Generic;

namespace SysCommand
{
    public interface IExecutionListener
    {
        void OnSuccess(string[] args, IEnumerable<CommandMap> map, Result<IMember> result);
        void OnNotFound(string[] args, IEnumerable<CommandMap> map, Result<IMember> result);
        void OnInvalidArgumentParse(string[] args, IEnumerable<CommandMap> map, Result<IMember> result);
        void OnError(string[] args, IEnumerable<CommandMap> map, Result<IMember> parseResult, Exception ex);
    }
}
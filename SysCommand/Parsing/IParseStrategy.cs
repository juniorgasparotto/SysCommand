using System.Collections.Generic;
using SysCommand.Mapping;
using SysCommand.Parsing;

namespace SysCommand.Parsing
{
    public interface IParseStrategy
    {
        ParseResult Parse(string[] args, IEnumerable<CommandMap> commandsMap, bool enableMultiAction);
    }
}
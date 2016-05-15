using System;

namespace SysCommand
{
    public interface IArguments
    {
        string Command { get; set; }
        string GetHelp(string propName);
    }
}

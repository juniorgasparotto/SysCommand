using System.Collections.Generic;

namespace SysCommand.ConsoleApp
{
    public abstract class Command : CommandBase
    {
        public App App { get; internal set; }
    }
}

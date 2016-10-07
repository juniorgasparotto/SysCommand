using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Reflection;

namespace SysCommand.ConsoleApp
{
    public sealed class AppEventsArgs
    {
        public App App { get; internal set; }
        public Result<IMember> Result { get; internal set; }
        public EvaluateState State { get; internal set; }
        public string[] Args { get; internal set; }

        internal AppEventsArgs() { }
    }
}

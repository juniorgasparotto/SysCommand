using SysCommand.Parser;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SysCommand
{
    public class CommandParseResult
    {
        public List<CommandParseLevelResult> Levels { get; set; }

        public bool HasError
        {
            get
            {
                return this.Levels.Any(f => f.MethodsInvalid.Any() || f.PropertiesInvalid.Any());
            }
        }

        public bool IsValid
        {
            get
            {
                return !HasError && this.Levels.Any(f => f.Methods.Any() || f.Properties.Any());
            }
        }

        public CommandBase Command { get; internal set; }
        public MethodMain MainResult { get; internal set; }

        public CommandParseResult()
        {
            this.Levels = new List<CommandParseLevelResult>();
        }

        public Result<IMember> GetResult()
        {
            var listAux = new List<IMember>();
            if (this.MainResult != null)
                listAux.Add(this.MainResult);
            listAux.AddRange(this.Levels.SelectMany(a => a.Result));
            return new Result<IMember>(listAux);
        }
    }
    
}
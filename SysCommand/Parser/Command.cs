using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace SysCommand
{
    public abstract class Command
    {
        public List<ActionMap> ActionsMaps { get; private set; }
        public List<ArgumentMap> ArgumentsMaps { get; private set; }

        public int OrderExecution { get; set; }
        public bool OnlyInDebug { get; set; }
        public bool UsePrefixInAllMethods { get; set; }
        public string PrefixMethods { get; set; }
        public bool OnlyMethodsWithAttribute { get; set; }
        public bool OnlyPropertiesWithAttribute { get; set; }

        public Command()
        {
            this.ActionsMaps.AddRange(CommandParser.GetActionsMapsFromType(this.GetType(), this.OnlyMethodsWithAttribute, this.UsePrefixInAllMethods, this.PrefixMethods));
            this.ArgumentsMaps.AddRange(CommandParser.GetArgumentsMapsFromProperties(this.GetType(), this.OnlyPropertiesWithAttribute));
        }

        public IEnumerable<ActionMapped> Parse(IEnumerable<ArgumentRaw> argsRaw, bool enableMultiAction)
        {
            return CommandParser.ParseActionMapped(argsRaw, enableMultiAction, this.ActionsMaps);
        }

        public IEnumerable<ArgumentMapped> Parse2(IEnumerable<ArgumentRaw> argsRaw, bool enablePositionalArgs)
        {
            return CommandParser.ParseArgumentMapped(argsRaw, enablePositionalArgs, this.ArgumentsMaps);
        }

    }
}

using System.Collections.Generic;
using SysCommand.Parsing;
using SysCommand.Execution;
using System;
using SysCommand.Mapping;

namespace SysCommand.DefaultExecutor
{
    public class Executor : IExecutor
    {
        public const string MAIN_METHOD_NAME = "main";
        private CommandMapper commandMapper;
        private ArgumentRawParser argumentRawParser;
        private ArgumentParser argumentParser;
        private ActionParser actionParser;

        public CommandMapper CommandMapper
        {
            get
            {
                if (commandMapper == null)
                {
                    var argMapper = new ArgumentMapper();
                    commandMapper = new CommandMapper(argMapper, new ActionMapper(argMapper));
                }
                return commandMapper;
            }
            set
            {
                commandMapper = value;
            }
        }

        public ArgumentRawParser ArgumentRawParser
        {
            get
            {
                if (argumentRawParser == null)
                    argumentRawParser = new ArgumentRawParser();
                return argumentRawParser;
            }
            set
            {
                argumentRawParser = value;
            }
        }

        public ArgumentParser ArgumentParser
        {
            get
            {
                if (argumentParser == null)
                    argumentParser = new ArgumentParser();
                return argumentParser;
            }
            set
            {
                argumentParser = value;
            }
        }

        public ActionParser ActionParser
        {
            get
            {
                if (actionParser == null)
                    actionParser = new ActionParser(this.ArgumentParser);
                return actionParser;
            }
            set
            {
                actionParser = value;
            }
        }

        public IEnumerable<CommandMap> GetMaps(IEnumerable<CommandBase> commands)
        {
            return this.CommandMapper.Map(commands);
        }

        public IEnumerable<ArgumentRaw> ParseRaw(string[] args, IEnumerable<CommandMap> commandsMap)
        {
            var allMethodsMaps = commandsMap.GetMethods();
            return this.ArgumentRawParser.Parse(args, allMethodsMaps);
        }

        public ParseResult Parse(string[] args, IEnumerable<ArgumentRaw> argumentsRaw, IEnumerable<CommandMap> commandsMap, bool enableMultiAction)
        {
            var parser = new InternalParser(this.ArgumentParser, this.ActionParser);
            return parser.Parse(args, argumentsRaw, commandsMap, enableMultiAction);
        }

        public ExecutionResult Execute(ParseResult parseResult, Action<IMemberResult, ExecutionScope> onInvoke)
        {
            var executor = new InternalExecutor();
            return executor.Execute(parseResult, onInvoke);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace SysCommand
{
    public class CommandMappedCollection : IEnumerable<CommandMappedCollection.CommandMapped>
    {
        private CommandMapCollection commandMapCollection;
        private List<CommandMapped> commandsMappeds;
        private IEnumerable<ActionMap> actionsMaps;
        private string[] args;
        private bool enableMultiAction;

        public IEnumerable<ArgumentRaw> ArgumentsRaws { get; private set; }
        public IEnumerable<ActionMapped> ActionsMappeds { get; private set; }

        public CommandMappedCollection(string[] args, CommandMapCollection commandMapCollection, bool enableMultiAction)
        {
            this.commandsMappeds = new List<CommandMapped>();
            this.args = args;
            this.enableMultiAction = enableMultiAction;
            this.actionsMaps = commandMapCollection.GetAllActionsMaps();
            this.commandMapCollection = commandMapCollection;

            this.ArgumentsRaws = CommandParser.ParseArgumentRaw(args, this.actionsMaps);
            this.ActionsMappeds = CommandParser.ParseActionMapped(this.ArgumentsRaws, enableMultiAction, this.actionsMaps);

            foreach (var commandMap in commandMapCollection)
                this.Add(commandMap.Command);
        }

        #region get command mapped

        public CommandMapped this[int index]
        {
            get
            {
                return commandsMappeds[index];
            }
        }

        public CommandMapped this[Type type]
        {
            get
            {
                return commandsMappeds.Where(f => f.Command.GetType() == type).FirstOrDefault();
            }
        }

        public CommandMapped this[Command command]
        {
            get
            {
                return commandsMappeds.Where(f => f.Command == command).FirstOrDefault();
            }
        }

        public CommandMapped Get<T>()
        {
            return commandsMappeds.FirstOrDefault(c => c.Command.GetType() == typeof(T));
        }

        #endregion

        private void Add(Command command)
        {
            var commandMapped = new CommandMapped(command);
            commandMapped.ArgumentsMappeds.AddRange(CommandParser.ParseArgumentMapped(this.ArgumentsRaws, command.EnablePositionalArgs, this.commandMapCollection[command].ArgumentsMaps));
            commandsMappeds.Add(commandMapped);
        }

        public T GetCommand<T>() where T : class
        {
            var commandMap = this.Get<T>();
            if (commandMap != null)
                return commandMap.Command as T;

            return null;
        }

        public IEnumerable<Command> GetAllCommands()
        {
            return commandsMappeds.Select(c => c.Command);
        }

        public IEnumerable<ArgumentMapped> GetAllArgumentsMappeds()
        {
            return commandsMappeds.SelectMany(c => c.ArgumentsMappeds);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.commandsMappeds.GetEnumerator();
        }

        public IEnumerator<CommandMapped> GetEnumerator()
        {
            return ((IEnumerable<CommandMapped>)commandsMappeds).GetEnumerator();
        }

        #region sub-classes

        public class CommandMapped
        {
            public Command Command { get; private set; }
            public List<ArgumentMapped> ArgumentsMappeds { get; private set; }

            internal CommandMapped(Command command)
            {
                this.Command = command;
                this.ArgumentsMappeds = new List<ArgumentMapped>();
            }
        }

        #endregion

    }
}

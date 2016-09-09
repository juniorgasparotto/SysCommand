using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace SysCommand
{
    public class CommandMapCollection : IEnumerable<CommandMapCollection.CommandMap>
    {
        private List<CommandMap> commandsMaps;

        private CommandMapCollection()
        {
            this.commandsMaps = new List<CommandMap>();
        }

        public CommandMapCollection(IEnumerable<Command> commands) : base()
        {
            foreach (var command in commands)
                this.Add(command);
        }

        #region get command maps

        public CommandMap this[int index]
        {
            get
            {
                return commandsMaps[index];
            }
        }

        public CommandMap this[Type type]
        {
            get
            {
                return commandsMaps.Where(f => f.Command.GetType() == type).FirstOrDefault();
            }
        }

        public CommandMap this[Command command]
        {
            get
            {
                return commandsMaps.Where(f => f.Command == command).FirstOrDefault();
            }
        }

        public CommandMap Get<T>()
        {
            return commandsMaps.FirstOrDefault(c => c.Command.GetType() == typeof(T));
        }
        
        #endregion

        private void Add(Command command)
        {
            var commandMaps = new CommandMap(command);
            commandMaps.ActionsMaps.AddRange(CommandParser.GetActionsMapsFromSourceObject(command.GetType(), command.OnlyMethodsWithAttribute, command.UsePrefixInAllMethods, command.PrefixMethods));
            commandMaps.ArgumentsMaps.AddRange(CommandParser.GetArgumentsMapsFromProperties(command.GetType(), command.OnlyPropertiesWithAttribute));
            commandsMaps.Add(commandMaps);
        }

        //public void Remove(Command command)
        //{
        //    commandsMaps.RemoveAll(c => c.Command == command);
        //}

        public T GetCommand<T>() where T : class
        {
            var commandMap = this.Get<T>();
            if (commandMap != null)
                return commandMap.Command as T;

            return null;
        }

        public IEnumerable<Command> GetAllCommands()
        {
            return commandsMaps.Select(c => c.Command);
        }

        public IEnumerable<ActionMap> GetAllActionsMaps()
        {
            return commandsMaps.SelectMany(c => c.ActionsMaps);
        }

        public IEnumerable<ArgumentMap> GetAllArgumentsMaps()
        {
            return commandsMaps.SelectMany(c => c.ArgumentsMaps);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.commandsMaps.GetEnumerator();
        }

        public IEnumerator<CommandMap> GetEnumerator()
        {
            return ((IEnumerable<CommandMap>)commandsMaps).GetEnumerator();
        }

        #region sub-classes

        public class CommandMap
        {
            public Command Command { get; private set; }
            public List<ActionMap> ActionsMaps { get; private set; }
            public List<ArgumentMap> ArgumentsMaps { get; private set; }

            internal CommandMap(Command command)
            {
                this.Command = command;
                this.ActionsMaps = new List<ActionMap>();
                this.ArgumentsMaps = new List<ArgumentMap>();
            }
        }

        #endregion
    }
}

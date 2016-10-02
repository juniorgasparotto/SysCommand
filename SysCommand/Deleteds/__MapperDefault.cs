//using System.Collections.Generic;
//using System.Linq;
//using System;
//using System.Collections;

//namespace SysCommand
//{
//    public class Mapped : IEnumerable<Mapped.CommandMapped>
//    {
//        private Map commandMapCollection;
//        private IEnumerable<ActionMap> actionsMaps;
//        private IEnumerable<ActionMapped> actionsMappeds;
//        private List<CommandMapped> commandsMappeds;
//        private string[] args;
//        private bool enableMultiAction;

//        public IEnumerable<ArgumentRaw> ArgumentsRaws { get; private set; }

//        public Mapped(string[] args, Map commandMapCollection, bool enableMultiAction)
//        {
//            this.commandsMappeds = new List<CommandMapped>();
//            this.args = args;
//            this.enableMultiAction = enableMultiAction;
//            this.commandMapCollection = commandMapCollection;

//            this.actionsMaps = this.commandMapCollection.GetAllActionsMaps();
//            this.ArgumentsRaws = CommandParser.ParseArgumentRaw(args, this.actionsMaps);
//            this.actionsMappeds = CommandParser.ParseActionMapped(this.ArgumentsRaws, enableMultiAction, this.actionsMaps);

//            foreach (var commandMap in commandMapCollection)
//                this.Add(commandMap.Command);
//        }

//        #region get command mapped

//        public CommandMapped this[int index]
//        {
//            get
//            {
//                return commandsMappeds[index];
//            }
//        }

//        public IEnumerable<CommandMapped> this[Type type]
//        {
//            get
//            {
//                return commandsMappeds.Where(f => f.Command.GetType() == type);
//            }
//        }

//        public CommandMapped this[object command]
//        {
//            get
//            {
//                return commandsMappeds.Where(f => f.Command == command).FirstOrDefault();
//            }
//        }

//        public IEnumerable<CommandMapped> Get<T>()
//        {
//            return commandsMappeds.Where(c => c.Command.GetType() == typeof(T));
//        }

//        #endregion

//        private void Add(Command command)
//        {
//            var commandMapped = new CommandMapped(command);
//            commandMapped.ArgumentsMappeds = CommandParser.ParseArgumentMapped(this.ArgumentsRaws, command.EnablePositionalArgs, this.commandMapCollection[command].ArgumentsMaps);
//            commandMapped.ActionsMappeds = this.actionsMappeds.Where(f => f.ActionMap.Source == command);
//            commandsMappeds.Add(commandMapped);
//        }

//        public IEnumerable<Command> GetAllCommands()
//        {
//            return commandsMappeds.Select(c => c.Command);
//        }

//        public IEnumerable<ActionMapped> GetAllActionsMappeds()
//        {
//            return this.actionsMappeds;
//        }

//        public IEnumerable<ArgumentMapped> GetAllArgumentsMappeds()
//        {
//            return commandsMappeds.SelectMany(c => c.ArgumentsMappeds);
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return this.commandsMappeds.GetEnumerator();
//        }

//        public IEnumerator<CommandMapped> GetEnumerator()
//        {
//            return ((IEnumerable<CommandMapped>)commandsMappeds).GetEnumerator();
//        }

//        #region sub-classes

//        public class CommandMapped
//        {
//            public Command Command { get; private set; }
//            public IEnumerable<ActionMapped> ActionsMappeds { get; internal set; }
//            public IEnumerable<ArgumentMapped> ArgumentsMappeds { get; internal set; }

//            internal CommandMapped(Command command)
//            {
//                this.Command = command;
//            }
//        }

//        #endregion

//    }
//}

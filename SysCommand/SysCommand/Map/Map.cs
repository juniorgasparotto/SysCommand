using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace SysCommand
{
    public class Map : IEnumerable<MapCommand>
    {
        private List<MapCommand> commandsMaps;

        public Map(IEnumerable<MapCommand> commands)
        {
            this.commandsMaps = commands.ToList();
        }

        #region get command maps

        public MapCommand this[int index]
        {
            get
            {
                return commandsMaps[index];
            }
        }

        public IEnumerable<MapCommand> this[Type type]
        {
            get
            {
                return commandsMaps.Where(f => f.Command.GetType() == type);
            }
        }

        public MapCommand this[object command]
        {
            get
            {
                return commandsMaps.Where(f => f.Command == command).FirstOrDefault();
            }
        }

        public IEnumerable<MapCommand> Get<T>()
        {
            return commandsMaps.Where(c => c.Command.GetType() == typeof(T));
        }
        
        #endregion

        private void Add(Command command)
        {
            var commandMaps = new MapCommand(command);
            commandMaps.Methods.AddRange(CommandParser.GetActionsMapsFromSourceObject(command, command.OnlyMethodsWithAttribute, command.UsePrefixInAllMethods, command.PrefixMethods));
            commandMaps.Properties.AddRange(CommandParser.GetArgumentsMapsFromProperties(command, command.OnlyPropertiesWithAttribute));
            commandsMaps.Add(commandMaps);
        }

        public IEnumerable<Command> GetAllCommands()
        {
            return commandsMaps.Select(c => c.Command);
        }

        public IEnumerable<ActionMap> GetAllActionsMaps()
        {
            return commandsMaps.SelectMany(c => c.Methods);
        }

        public IEnumerable<ArgumentMap> GetAllArgumentsMaps()
        {
            return commandsMaps.SelectMany(c => c.Properties);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.commandsMaps.GetEnumerator();
        }

        public IEnumerator<MapCommand> GetEnumerator()
        {
            return ((IEnumerable<MapCommand>)commandsMaps).GetEnumerator();
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using SysCommand.Parser;

namespace SysCommand
{
    public class Map : IEnumerable<MapItem>
    {
        private List<MapItem> commandsMaps;

        public Map(IEnumerable<MapItem> commands)
        {
            this.commandsMaps = commands.ToList();
        }

        #region get command maps

        public MapItem this[int index]
        {
            get
            {
                return commandsMaps[index];
            }
        }

        public IEnumerable<MapItem> this[Type type]
        {
            get
            {
                return commandsMaps.Where(f => f.Command.GetType() == type);
            }
        }

        public MapItem this[object command]
        {
            get
            {
                return commandsMaps.Where(f => f.Command == command).FirstOrDefault();
            }
        }

        public IEnumerable<MapItem> Get<T>()
        {
            return commandsMaps.Where(c => c.Command.GetType() == typeof(T));
        }
        
        #endregion
                
        public IEnumerable<Command> GetCommands()
        {
            return commandsMaps.Select(c => c.Command);
        }

        public IEnumerable<ActionMap> GetMethods()
        {
            return commandsMaps.SelectMany(c => c.Methods);
        }

        public IEnumerable<ArgumentMap> GetProperties()
        {
            return commandsMaps.SelectMany(c => c.Properties);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.commandsMaps.GetEnumerator();
        }

        public IEnumerator<MapItem> GetEnumerator()
        {
            return ((IEnumerable<MapItem>)commandsMaps).GetEnumerator();
        }
    }
}

//using SysCommand.Parsing;
//using SysCommand.Utils;
//using System.Collections.Generic;

//namespace SysCommand.Mapping
//{
//    public class CommandMapper
//    {
//        public IEnumerable<CommandMap> Map(IEnumerable<CommandBase> commands)
//        {
//            foreach (var command in commands)
//                yield return this.CreateMap(command);
//        }

//        private CommandMap CreateMap(CommandBase command)
//        {
//            var mapCommand = new CommandMap(command);
//            mapCommand.Methods.AddRange(CommandParserUtils.GetActionsMapsFromTargetObject(command, command.OnlyMethodsWithAttribute, command.UsePrefixInAllMethods, command.PrefixMethods));
//            mapCommand.Properties.AddRange(CommandParserUtils.GetArgumentsMapsFromProperties(command, command.OnlyPropertiesWithAttribute));
//            return mapCommand;
//        }
//    }
//}

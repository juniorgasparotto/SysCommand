using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysCommand.Mapping;

namespace SysCommand.ConsoleApp.Commands
{
    public class HelpCommand : Command, IHelpCommand
    {
        public HelpCommand()
        {
            HelpText = "displays help information";
        }

        public string Help(string action = null)
        {
            if (action == null)
                return this.App.Descriptor.GetHelpText(this.App.Maps);
            else
                return this.App.Descriptor.GetHelpText(this.App.Maps, action);

            //var help = new Commands.User.Help.help();
            //help.Session = new Dictionary<string, object>();
            //help.Session.Add("Model", this.App.Maps);
            //help.Initialize();
            //return help.TransformText();
            //return ViewT4<Commands.User.Help.HelpTemplate, IEnumerable<CommandMap>>(this.App.Maps);

            //return View<IEnumerable<CommandMap>>(null);
            return View(this.App.Maps);

            var strBuilder = new StringBuilder();
            foreach (var map in this.App.Maps)
            {
                strBuilder.AppendLine(map.Command.GetType().Name + ":");

                var dicProperty = new Dictionary<string, string>();
                foreach (var arg in map.Properties)
                {
                    var key = this.GetArgumentNameWithPrefix(arg);
                    dicProperty[key] = arg.HelpText;
                }

                if (dicProperty.Count > 0)
                {
                    strBuilder.Append(ConsoleAppHelper.GetConsoleHelper(dicProperty, 4));
                    if (map.Methods.Any())
                        strBuilder.AppendLine();
                }

                foreach (var method in map.Methods)
                {
                    var dic = new Dictionary<string, string>();
                    var defaultHelp = method.IsDefault ? "[default], " : "";

                    strBuilder.AppendLine(string.Format("    {0}{1}", defaultHelp, method.ActionName));

                    foreach (var arg in method.ArgumentsMaps)
                    {
                        var key = this.GetArgumentNameWithPrefix(arg);
                        dic[key] = arg.HelpText;
                    }

                    if (dic.Count > 0)
                        strBuilder.Append(ConsoleAppHelper.GetConsoleHelper(dic, 10));
                }
            }

            //return strBuilder;
        }

        private string GetArgumentNameWithPrefix(ArgumentMap arg)
        {
            string key = null;
            var shortName = arg.ShortName != null ? arg.ShortName.ToString() : null;
            if (!string.IsNullOrWhiteSpace(shortName) && !string.IsNullOrWhiteSpace(arg.LongName))
                key = "-" + arg.ShortName + ", --" + arg.LongName;
            else if (!string.IsNullOrWhiteSpace(shortName))
                key = "-" + arg.ShortName;
            else if (!string.IsNullOrWhiteSpace(arg.LongName))
                key = "--" + arg.LongName;

            return key;
        }

    }
}

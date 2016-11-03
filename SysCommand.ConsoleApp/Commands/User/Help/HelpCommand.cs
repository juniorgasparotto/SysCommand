using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysCommand.Mapping;
using SysCommand.ConsoleApp.View;
using SysCommand.DefaultExecutor;
using System;

namespace SysCommand.ConsoleApp
{
    public class HelpCommand : Command, IHelpCommand
    {
        public string Help(string action = null, string command = null)
        {
            var table = new TableView();
            table.AddLineSeparator = true;
            table.AddColumnSeparator = true;

            table.PaddingLeft = 2;
            table.PaddingTop = 1;
            table.PaddingBottom = 2;
            table.IncludeHeader = true;
            table.AddColumnDefinition("A", 10, 0, 2);
            table.AddColumnDefinition("B", 10);
            table.AddColumnDefinition("C", 10);
            table.AddColumnDefinition("D", 10);

            table.AddRow()
                .AddColumnInRow("0000000000000000000000")
                .AddColumnInRow("111111111111 11111111111111 1111")
                .AddColumnInRow("22222222222 222222222 222222222 2222222")
                .AddColumnInRow("33333333333333333333333333 333333333333 333 3 3 3333 3");

            table.AddRow()
                .AddColumnInRow("4444000000000")
                .AddColumnInRow("5555 11111111111111 1111")
                .AddColumnInRow("66666 222222222 222222222 2222222")
                .AddColumnInRow("777777 333333333333 333 3 3 3333 3");

            table.AddRow()
                .AddColumnInRow("88888888888800")
                .AddColumnInRow("AAAAAA AAAAAAAAA AAAAAAAAAAAA")
                .AddColumnInRow("CCCCCCCCC 222222222 222222222 2222222")
                .AddColumnInRow("It is a long established fact that a reader It is a long established fact that a reader");

            table.Build();
            return table.ToString();

            return this.App.Descriptor.GetHelpText(this.App.Maps);

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

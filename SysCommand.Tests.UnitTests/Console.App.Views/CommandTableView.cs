using SysCommand.ConsoleApp;
using SysCommand.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysCommand.Tests.UnitTests
{
    public class CommandTableView : Command
    {
        public CommandTableView()
        {
            this.HelpText = "command help";
        }

        [Argument(Help = "Give the output in the short-format.", ShortName = 's', LongName = "short", ShowHelpComplement = true, DefaultValue = true)]
        public bool IsShort { get; set; }

        [Argument(Help = "Show the branch and tracking info even in short-format.", ShortName = 'b', LongName = "branch", ShowHelpComplement = true, DefaultValue = null)]
        public string Branch { get; set; }

        [Argument(Help = "Give the output in an easy-to-parse format for scripts. This is similar to the short output, but will remain stable across Git versions and regardless of user configuration. See below for details.", LongName = "porcelain", ShowHelpComplement = true, DefaultValue = null)]
        public string Porcelain { get; set; }

        [Argument(Help = "In addition to the names of files that have been changed,", ShortName = 'v', ShowHelpComplement = true, DefaultValue = Verbose.All)]
        public Verbose Verbose { get; set; }

        [Action(Help="action help")]
        public string Status()
        {
            return "";
        }
    }
}

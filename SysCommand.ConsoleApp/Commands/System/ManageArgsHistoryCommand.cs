using SysCommand.ConsoleApp.Files;
using System.Collections.Generic;

namespace SysCommand.ConsoleApp.Commands
{
    public class ManageArgsHistoryCommand : Command, IManageArgsHistoryCommand
    {
        public string CmdName { get; set; }
        public string CmdSave { get; set; }
        public string CmdDelete { get; set; }

        public string[] Main()
        {
            var jsonFiles = App.Items.GetOrCreate<JsonFiles>();
            var histories = jsonFiles.GetOrCreate<List<History>>("histories");
            histories.RemoveAll(f => f.Name == CmdName);
            histories.Add(new History
            {
                Name = CmdName,
                Args = ExecutionScope.ParseResult.Args
            });
            jsonFiles.Save(histories, @"histories", false);
            return ExecutionScope.ParseResult.Args;
        }

        public class History
        {
            public string Name { get; set; }
            public string[] Args { get; set; }
        }
    }
}

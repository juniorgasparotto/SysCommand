using SysCommand.ConsoleApp.Files;
using SysCommand.ConsoleApp.Results;
using SysCommand.ConsoleApp.View;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysCommand.ConsoleApp.Commands
{
    public class ArgsHistoryCommand : Command
    {
        public const string FILE_NAME = "history";
        private JsonFileManager fileManager;

        public ArgsHistoryCommand()
        {
            this.fileManager = App.Items.GetOrCreate<JsonFileManager>();
        }

        public RestartResult HistoryLoad(string name)
        {
            var histories = this.fileManager.GetOrCreate<List<History>>(FILE_NAME);
            var history = histories.FirstOrDefault(f => f.Name == name);
            string[] args = null;
            if (history != null)
                args = history.Args.ToArray();
            return new RestartResult(args);
        }

        public void HistorySave(string name)
        {
            var histories = this.fileManager.GetOrCreate<List<History>>(FILE_NAME);
            histories.RemoveAll(f => f.Name == name);

            var args = ExecutionScope.ParseResult.Args.ToList();
            var actionMapName = this.CurrentActionMap().ActionName;
            var curMethodArg = args.FirstOrDefault(f => f == actionMapName);
            if (curMethodArg != null)
            {
                var index = args.IndexOf(curMethodArg);
                // remove "name" value
                args.RemoveAt(index + 1);
                // remove action name
                args.Remove(curMethodArg);
            }

            if (args.Count > 0)
            {
                histories.Add(new History
                {
                    Name = name,
                    Args = args
                });

                fileManager.Save(histories, FILE_NAME);
            }
        }

        public string HistoryList()
        {
            var strBuilder = new StringBuilder();
            var histories = this.fileManager.GetOrCreate<List<History>>(FILE_NAME);
            var table = new TableView();
            table.AddLineSeparator = false;

            foreach (var history in histories)
            {
                table.AddRowSummary("[" + history.Name + "] " + string.Join(" ", history.Args));
            }

            //table.AddLineSeparator = false;
            //table.AddColumnSeparator = true;
            //table.IncludeHeader = true;
            //table.AddColumnDefinition("Name", 0, 0, 3);
            //table.AddColumnDefinition("Args", 50);

            //foreach (var history in histories)
            //{
            //    table.AddRow()
            //        .AddColumnInRow(history.Name)
            //        .AddColumnInRow(string.Join(" ", history.Args));
            //}

            return table
                .Build()
                .ToString();
        }

        public void HistoryDel(string name)
        {
            var histories = this.fileManager.GetOrCreate<List<History>>(FILE_NAME);
            histories.RemoveAll(f => f.Name == name);
            fileManager.Save(histories, FILE_NAME);
        }

        public class History
        {
            public string Name { get; set; }
            public IEnumerable<string> Args { get; set; }
        }
    }
}

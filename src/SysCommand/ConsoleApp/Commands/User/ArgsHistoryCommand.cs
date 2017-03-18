using SysCommand.ConsoleApp.Files;
using SysCommand.ConsoleApp.Results;
using SysCommand.ConsoleApp.View;
using SysCommand.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysCommand.ConsoleApp.Commands
{
    public class ArgsHistoryCommand : Command
    {
        public const string FILE_NAME = "history";

        private JsonFileManager FileManager
        {
            get
            {
                return App.Items.GetOrCreate<JsonFileManager>();
            }
        }

        public ArgsHistoryCommand()
        {
            this.HelpText = "Action for argument management";
        }

        public RedirectResult HistoryLoad(string name)
        {
            var histories = this.FileManager.GetOrCreate<List<History>>(FILE_NAME);
            var history = histories.FirstOrDefault(f => f.Name == name);
            if (history == null)
                 throw new Exception(string.Format("The history name '{0}' dosen't exists", name));

            return new RedirectResult(history.Args.ToArray());
        }

        public void HistorySave(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            var histories = this.FileManager.GetOrCreate<List<History>>(FILE_NAME);
            histories.RemoveAll(f => f.Name == name);

#if NETCORE
            var actionMapName = "history-save";
            var parameterName = "--name";
#else
            var actionMap = this.GetActionMap();
            var actionMapName = actionMap.ActionName;
            var parameterName = "--" + actionMap.ArgumentsMaps.ElementAt(0).LongName;
#endif
            var newArgs = new List<string>();
            var argEnumerator = ExecutionScope.ParseResult.Args.ToList().GetEnumerator();

            while (argEnumerator.MoveNext())
            {
                var arg = argEnumerator.Current;
                if (arg == actionMapName)
                {
                    argEnumerator.MoveNext();
                    if (argEnumerator.Current == parameterName)
                        argEnumerator.MoveNext();
                    continue;
                }
                newArgs.Add(arg);
            }

            if (newArgs.Count > 0)
            {
                histories.Add(new History
                {
                    Name = name,
                    Args = newArgs
                });

                FileManager.Save(histories, FILE_NAME);
            }
        }

        public string HistoryList()
        {
            var strBuilder = new StringBuilder();
            var histories = this.FileManager.GetOrCreate<List<History>>(FILE_NAME);
            var table = new TableView();
            table.AddLineSeparator = false;

            foreach (var history in histories)
            {
                var newArgs = history.Args.Select(arg => ArgumentParsed.GetValueRaw(arg));
                table.AddRowSummary("[" + history.Name + "] " + string.Join(" ", newArgs));
            }

            return table
                .Build()
                .ToString();
        }

        public void HistoryDelete(string name)
        {
            var histories = this.FileManager.GetOrCreate<List<History>>(FILE_NAME);
            var history = histories.FirstOrDefault(f => f.Name == name);
            if (history == null)
                throw new Exception(string.Format("The history name '{0}' dosen't exists"));

            histories.RemoveAll(f => f.Name == name);
            FileManager.Save(histories, FILE_NAME);
        }

        public class History
        {
            public string Name { get; set; }
            public IEnumerable<string> Args { get; set; }
        }
    }
}

using Fclp;
using System;

namespace SysCommand
{
    [CommandClassAttribute(OrderExecution = -1000)]
    public class ManagerCommand : Command<ManagerCommand.Arguments>
    {
        public ManagerCommand()
        {
            this.IgnoreSaveInHistory = true;
        }

        public override void Execute()
        {
            if (this.Args.ShowAll)
                this.Show();

            if (!string.IsNullOrWhiteSpace(this.Args.Show))
                this.Show(this.Args.Show);

            if (this.Args.Save)
                this.Save();

            if (this.Args.Remove)
                this.Remove();
        }

        public void Save()
        {
            if (!App.Current.GetObjectFile<ArgumentsHistory>().Object.CommandsHistories.ContainsKey(App.Current.CurrentCommandName))
            {
                Console.WriteLine("The command has no argument to save.");
                App.Current.StopPropagation();
                return;
            }

            App.Current.GetObjectFile<ArgumentsHistory>().Save();
            App.Current.StopPropagation();
            Console.WriteLine("The command '{0}' was successfully saved", App.Current.CurrentCommandName);
        }

        public void Remove()
        {
            if (!App.Current.GetObjectFile<ArgumentsHistory>().Object.CommandsHistories.ContainsKey(App.Current.CurrentCommandName))
            {
                Console.WriteLine("Command name '{0}' dosen't exists", App.Current.CurrentCommandName);
                App.Current.StopPropagation();
                return;
            }

            App.Current.GetObjectFile<ArgumentsHistory>().Object.DeleteCommand(App.Current.CurrentCommandName);
            App.Current.GetObjectFile<ArgumentsHistory>().Save();
            App.Current.StopPropagation();
            Console.WriteLine("The command '{0}' was successfully removed.", App.Current.CurrentCommandName);
        }

        public void Show()
        {
            if (App.Current.GetObjectFile<ArgumentsHistory>().Object.CommandsHistories.Count == 0)
            {
                Console.WriteLine("No command was found to display.");
                App.Current.StopPropagation();
                return;
            }

            foreach (var commandKeyValue in App.Current.GetObjectFile<ArgumentsHistory>().Object.CommandsHistories)
            {   
                var argsOutput = "";
                foreach (var args in commandKeyValue.Value)
                {
                    argsOutput += argsOutput == "" ? args.Value.Command : " " + args.Value.Command;
                }

                Console.WriteLine("\"{0}\" {1}", commandKeyValue.Key, argsOutput);
            }

            App.Current.StopPropagation();
        }

        public void Show(string commandName)
        {
            if (!App.Current.GetObjectFile<ArgumentsHistory>().Object.CommandsHistories.ContainsKey(commandName))
            {
                Console.WriteLine("Command name '{0}' dosen't exists", commandName);
                App.Current.StopPropagation();
                return;
            }

            var command = App.Current.GetObjectFile<ArgumentsHistory>().Object.CommandsHistories[commandName];
            var argsOutput = "";
            foreach (var args in command.Values)
            {
                argsOutput += argsOutput == "" ? args.Command : " " + args.Command;
            }

            Console.WriteLine("\"{0}\" {1}", commandName, argsOutput);
            App.Current.StopPropagation();
        }

        #region Internal Parameters
        public class Arguments
        {
            [CommandPropertyAttribute(LongName = "cmd-save", Help = "Save current command to the history commands")]
            public bool Save { get; set; }

            [CommandPropertyAttribute(LongName = "cmd-remove", Help = "Remove current command from the history commands")]
            public bool Remove { get; set; }

            [CommandPropertyAttribute(LongName = "cmd-show-all", Help = "Show all commands from the history.")]
            public bool ShowAll { get; set; }

            [CommandPropertyAttribute(LongName = "cmd-show", Help = "Show the specific command, if exists, from the history.")]
            public string Show { get; set; }
        }
        #endregion
    }
}

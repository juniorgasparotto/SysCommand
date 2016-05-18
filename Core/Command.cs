using Fclp;
using System;

namespace SysCommand
{
    public abstract class Command<TArgs> : ICommand
    {
        public bool HasParsed { get; protected set; }
        public bool HasLoadedFromConfig { get; protected set; }
        public bool IgnoreSaveInHistory { get; protected set; }

        public TArgs Args { get; protected set; }
        public FluentCommandLineParser Parser { get; protected set; }
        public ICommandLineParserResult ParserResult { get; protected set; }

        public virtual void ParseArgs(string[] args)
        {
            var itemHistory = App.Current.GetObjectFile<ArgumentsHistory>().GetCommandArguments(App.Current.CurrentCommandName, typeof(TArgs));
            if (itemHistory == null || itemHistory.Object == null)
            {
                this.Args = Activator.CreateInstance<TArgs>();
                itemHistory = itemHistory ?? new ArgumentsHistory.ArgumentsHistoryItem();
                itemHistory.Object = this.Args;
            }
            else
            {
                this.HasLoadedFromConfig = true;
                this.Args = (TArgs)itemHistory.Object;
            }

            this.Parser = new FluentCommandLineParser();
            var autoFill = new CommandPropertyAutoFill(this.Parser, this.Args, args);
            autoFill.AutoSetup();

            this.ParserResult = this.Parser.Parse(args);

            if (this.ParserResult.HasErrors == false && autoFill.HasFill)
            {
                this.HasParsed = true;
                
                // Only update 'Args.Command' and set the args in config if has success
                if (!this.IgnoreSaveInHistory)
                {
                    itemHistory.Command = autoFill.GetCommandsParsed();
                    App.Current.GetObjectFile<ArgumentsHistory>().SetCommandArguments(App.Current.CurrentCommandName, itemHistory);
                }
            }
        }

        public abstract void Execute();
    }
}

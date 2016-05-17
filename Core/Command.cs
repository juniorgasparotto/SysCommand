using Fclp;
using System;

namespace SysCommand
{
    public abstract class Command<TArgs> : ICommand where TArgs : class, IArguments
    {
        public bool HasParsed { get; protected set; }
        public bool HasLoadedFromConfig { get; protected set; }
        public bool IgnoreSaveInHistory { get; protected set; }

        public TArgs Args { get; protected set; }
        public FluentCommandLineParser Parser { get; protected set; }
        public ICommandLineParserResult ParserResult { get; protected set; }

        public virtual void ParseArgs(string[] args)
        {
            this.Args = App.Current.GetConfig<ArgumentsHistory>().GetCommandArguments(App.Current.CurrentCommandName, typeof(TArgs)) as TArgs;
            if (this.Args == null)
            {
                this.Args = Activator.CreateInstance<TArgs>();
            }
            else
            {
                this.HasLoadedFromConfig = true;
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
                    autoFill.UpdateUsedCommandsInArgs();
                    App.Current.GetConfig<ArgumentsHistory>().SetCommandArguments(App.Current.CurrentCommandName, this.Args);
                }
            }
        }

        public abstract void Execute();
    }
}

using Fclp;
using System;

namespace SysCommand
{
    public abstract class Command<TArgs> : ICommand where TArgs : class, IArguments
    {
        public bool HasParsed { get; protected set; }
        public TArgs Args { get; protected set; }
        public FluentCommandLineParser Parser { get; protected set; }
        public ICommandLineParserResult ParserResult { get; protected set; }

        public virtual void ParseArgs(string[] args)
        {
            this.Args = App.CurrentConfiguration.GetCommandParameters(App.CurrentCommandName, typeof(TArgs)) as TArgs;
            if (this.Args == null)
            {
                this.Args = Activator.CreateInstance<TArgs>();
                App.CurrentConfiguration.SetCommandParameters(App.CurrentCommandName, this.Args);
            }

            this.Parser = new FluentCommandLineParser();
            var autoFill = new CommandPropertyAutoFill(this.Parser, this.Args, args);
            autoFill.AutoSetup();

            this.ParserResult = this.Parser.Parse(args);

            if (this.ParserResult.HasErrors == false && autoFill.HasFill)
                this.HasParsed = true;
        }

        public abstract void Execute();
    }
}

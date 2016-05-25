using Fclp;
using System;

namespace SysCommand
{
    public abstract class CommandArguments<TArgs> : ICommand
    {
        public bool HasParsed { get; protected set; }
        public bool HasLoadedFromConfig { get; protected set; }
        
        public TArgs ArgsObject { get; protected set; }
        public string[] Args { get; protected set; }

        public FluentCommandLineParser Parser { get; protected set; }
        public ICommandLineParserResult ParserResult { get; protected set; }

        public bool AllowSaveArgsInStorage { get; protected set; }
        public int OrderExecution { get; set; }
        public bool OnlyInDebug { get; set; }

        public virtual void Load(string[] args)
        {
            this.Args = args;
        }

        public virtual void Parse()
        {
            if (this.AllowSaveArgsInStorage)
                this.ParseStored(this.Args);
            else
                this.ParseNoStored(this.Args);
        }

        private void ParseStored(string[] args)
        {
            if (this.HasParsed)
                return;

            var itemHistory = App.Current.GetOrCreateObjectFile<CommandStorage>().GetArguments(App.Current.CurrentCommandName, typeof(TArgs));
            if (itemHistory == null || itemHistory.Object == null)
            {
                this.ArgsObject = Activator.CreateInstance<TArgs>();
                itemHistory = itemHistory ?? new CommandStorage.Item();
                itemHistory.Object = this.ArgsObject;
            }
            else
            {
                this.HasLoadedFromConfig = true;
                this.ArgsObject = (TArgs)itemHistory.Object;
            }

            this.Parser = new FluentCommandLineParser();
            var autoFill = new CommandPropertyAutoFill(this.Parser, this.ArgsObject, args);
            autoFill.AutoSetup();

            this.ParserResult = this.Parser.Parse(args);

            if (this.ParserResult.HasErrors == false && autoFill.HasFill)
            {
                this.HasParsed = true;
                
                // Only update 'Args.Command' and set the args in config if has success
                if (this.AllowSaveArgsInStorage)
                {
                    itemHistory.Command = autoFill.GetCommandsParsed();
                    App.Current.GetOrCreateObjectFile<CommandStorage>().SetArguments(App.Current.CurrentCommandName, itemHistory);
                }
            }
        }

        private void ParseNoStored(string[] args)
        {
            this.ArgsObject = Activator.CreateInstance<TArgs>();
            this.Parser = new FluentCommandLineParser();
            var autoFill = new CommandPropertyAutoFill(this.Parser, this.ArgsObject, args);
            autoFill.AutoSetup();
            this.ParserResult = this.Parser.Parse(args);

            if (this.ParserResult.HasErrors == false && autoFill.HasFill)
                this.HasParsed = true;
        }
        
        public abstract void Execute();
    }
}

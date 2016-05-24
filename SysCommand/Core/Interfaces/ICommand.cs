using Fclp;
using System;

namespace SysCommand
{
    public interface ICommand
    {
        bool HasParsed { get; }
        bool HasLoadedFromConfig { get; }
        FluentCommandLineParser Parser { get; }
        ICommandLineParserResult ParserResult { get; }
        
        #region config

        bool AllowSaveArgsInStorage { get; }
        int OrderExecution { get; }
        bool OnlyInDebug { get; }

        #endregion

        #region Methods in sequence
        void Load(string[] args);
        void Parse();
        void Execute();
        #endregion
    }
}

using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;

namespace SysCommand
{
    public class App
    {
        private List<Type> IgnoredCommands = new List<Type>();
        private Dictionary<string, Config> Configs = new Dictionary<string,Config>();
        private List<ICommand> Commands { get; set; }
        private const string COMMAND_NAME_DEFAULT = "default";

        public bool DebugShowArgsInput = true;
        public bool DebugShowExitConfirm = true;
        public bool DebugSaveConfigsInRootFolder = true;

        public string CurrentCommandName { get; set; }
        public bool InStopPropagation { get; private set; }

        public static App Current { get; private set; }

        protected App() { }

        public static void Initialize()
        {
            App.Current = new App();
        }

        public virtual void Run()
        {            
            var args = Environment.GetCommandLineArgs();

#if DEBUG
            if (DebugShowArgsInput)
            {
                Console.WriteLine("Enter with args:");
                args = AppHelpers.StringToArgs(Console.ReadLine());
            }
#endif

            // retorna o nome do comando ou utiliza o padrão
            this.CurrentCommandName = App.COMMAND_NAME_DEFAULT;
            if (args != null && args.Length > 0 && args[0][0] != '-')
                this.CurrentCommandName = args[0];

            LoadCommands();
            ParseArgsAll(args);

            if (ValidateParserErrors())
                ExecuteAll();

            if (DebugShowExitConfirm)
                this.ExitWithKeyEnterInDebug();
        }

        protected virtual void LoadCommands()
        {
            var listOfCommands = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                         typeof(ICommand).IsAssignableFrom(assemblyType)
                                      && assemblyType.IsInterface == false
                                      && assemblyType.IsAbstract == false
                                  select new
                                  {
                                      type = assemblyType,
                                      attr = assemblyType.GetCustomAttributes(typeof(CommandClassAttribute), true).FirstOrDefault() as CommandClassAttribute
                                  }).ToList();

            listOfCommands = listOfCommands.OrderBy(f => f.attr == null ? 0 : f.attr.OrderExecution).ToList();
            this.Commands = listOfCommands.Select(f => (ICommand)Activator.CreateInstance(f.type)).ToList();
            this.Commands.RemoveAll(f => IgnoredCommands.Contains(f.GetType()));
        }

        protected virtual void ParseArgsAll(string[] args)
        {
            foreach (var cmd in this.Commands)
                cmd.ParseArgs(args);
        }

        protected virtual bool ValidateParserErrors()
        {
            var hasError = false;

            foreach (var cmd in this.Commands)
            {
                if (cmd.ParserResult.HasErrors)
                {
                    hasError = true;
                    Console.WriteLine(cmd.ParserResult.ErrorText);
                }
            }

            if (hasError)
                ShowHelp();

            return !hasError;
        }

        protected virtual void ExecuteAll()
        {
            foreach (var cmd in this.Commands)
            {
                if ((cmd.HasParsed || cmd.HasLoadedFromConfig) && !this.InStopPropagation)
                    cmd.Execute();

                if (this.InStopPropagation)
                {
                    this.InStopPropagation = false;
                    break;
                }
            }
        }

        private void ExitWithKeyEnterInDebug()
        {
#if DEBUG
            Console.WriteLine("Press ENTER to Exit");
            Console.ReadLine();
#endif
        }

        public virtual void Exit(int code)
        {
            this.ExitWithKeyEnterInDebug();
            Environment.Exit(code);
        }

        public virtual void StopPropagation()
        {
            this.InStopPropagation = true;
        }

        public virtual void ShowHelp()
        {
            var dic = new Dictionary<string, string>();
            foreach (var cmd in this.Commands)
            {
                foreach (var opt in cmd.Parser.Options)
                {
                    var key = "";
                    if (!string.IsNullOrWhiteSpace(opt.ShortName) && !string.IsNullOrWhiteSpace(opt.LongName))
                        key = "-" + opt.ShortName + ", --" + opt.LongName;
                    else if (!string.IsNullOrWhiteSpace(opt.ShortName))
                        key = "-" + opt.ShortName;
                    else if (!string.IsNullOrWhiteSpace(opt.LongName))
                        key = "--" + opt.LongName;

                    dic[key] = opt.Description;
                }
            }

            Console.WriteLine(AppHelpers.GetConsoleHelper(dic));
        }

        public virtual TConfig GetConfig<TConfig>(string fileName = null, bool refresh = false) where TConfig : Config
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = GetConfigNameDefault(typeof(TConfig));

            if (Configs.ContainsKey(fileName) && !refresh)
                return (TConfig)Configs[fileName];

            var fileName2 = fileName;
            if (this.DebugSaveConfigsInRootFolder)
            {
#if DEBUG
                fileName2 = @"..\..\" + fileName2;
#endif
            }

            var config = Config.Get<TConfig>(fileName2);
            Configs[fileName] = config;

            return config;
        }

        public void IgnoreCommmand<T>()
        {
            IgnoredCommands.Add(typeof(T));
        }

        private string GetConfigNameDefault(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(ConfigClassAttribute), true).FirstOrDefault() as ConfigClassAttribute;
            if (attr != null && !string.IsNullOrWhiteSpace(attr.FileName))
                return attr.FileName;

            var name = Char.ToLowerInvariant(type.Name[0]) + type.Name.Substring(1);
            var configName = "syscmd." + name + ".config";
            configName = System.Text.RegularExpressions.Regex.Replace(configName, @"(?<!_)([A-Z])", ".$1").ToLower();
            return configName;
        }
    }
}

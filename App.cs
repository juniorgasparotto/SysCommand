using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;

namespace SysCommand
{
    public static class App
    {
        public const string COMMAND_NAME_DEFAULT = "default";
        public static Configuration CurrentConfiguration { get; set; }
        public static string CurrentCommandName { get; set; }
        public static List<ICommand> Commands { get; private set; }

        public static void Init()
        {
            //try
            {
                var args = Environment.GetCommandLineArgs();
                
#if DEBUG
                Console.WriteLine("Entre com os argumentos:");
                args = AppHelpers.StringToArgs(Console.ReadLine());
#endif

                // Seta o paramêtro default do site de acordo com o que esta salvo nas configurações
                App.CurrentConfiguration = Configuration.Get();

                // retorna o nome do comando ou utiliza o padrão
                App.CurrentCommandName = App.COMMAND_NAME_DEFAULT;
                if (args != null && args.Length > 1 && args[0][0] != '-')
                    App.CurrentCommandName = args[0];

                LoadCommands();
                ParseArgsAll(args);

                if (ValidateParserErrors())
                    ExecuteAll();
            }
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Ocorreu um erro inesperado, digite --help para solicitar ajuda: " + ex.Message);
            //}

            // confirma saida em debug
            App.ExitWithKeyEnterInDebug();
        }

        private static void LoadCommands()
        {
            var listOfCommands = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where typeof(ICommand).IsAssignableFrom(assemblyType) && assemblyType.IsInterface == false && assemblyType.IsAbstract == false
                                  select new
                                  {
                                      type = assemblyType,
                                      attr = assemblyType.GetCustomAttributes(typeof(CommandClassAttribute), true).FirstOrDefault() as CommandClassAttribute
                                  }).ToList();

            listOfCommands = listOfCommands.OrderBy(f => f.attr == null ? 0 : f.attr.OrderExecution).ToList();
            App.Commands = listOfCommands.Select(f => (ICommand)Activator.CreateInstance(f.type)).ToList();
        }

        private static void ParseArgsAll(string[] args)
        {
            foreach (var cmd in App.Commands)
                cmd.ParseArgs(args);
        }

        private static bool ValidateParserErrors()
        {
            var hasError = false;

            foreach (var cmd in App.Commands)
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

        private static void ExecuteAll()
        {
            foreach (var cmd in App.Commands)
                if (cmd.HasParsed)
                    cmd.Execute();
        }


        private static void ExitWithKeyEnterInDebug()
        {
#if DEBUG
            Console.WriteLine("Tecle qualquer tecla para sair");
            Console.Read();
#endif
        }

        public static void Exit(int code)
        {
            // confirma saida em debug
            App.ExitWithKeyEnterInDebug();
            Environment.Exit(code);
        }

        public static void ShowHelp()
        {
            var dic = new Dictionary<string, string>();
            foreach (var cmd in App.Commands)
            {
                foreach (var opt in cmd.Parser.Options)
                {
                    var key = string.IsNullOrWhiteSpace(opt.ShortName) ? "--" + opt.LongName : "-" + opt.ShortName + ", --" + opt.LongName;
                    dic[key] = opt.Description;
                }
            }

            Console.WriteLine(AppHelpers.GetConsoleHelper(dic));
        }
    }
}

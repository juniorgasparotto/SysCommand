using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;
using System.IO;

namespace SysCommand
{
    public class App
    {
        private const string COMMAND_NAME_DEFAULT = "default";
        private List<Type> IgnoredCommands = new List<Type>();
        private Dictionary<string, object> ObjectsFilesLoadeds = new Dictionary<string, object>();
        private List<ICommand> Commands { get; set; }

        public string CurrentCommandName { get; private set; }
        public bool InStopPropagation { get; private set; }

        public bool DebugShowArgsInput { get; set; }
        public bool DebugShowExitConfirm { get; set; }
        public bool DebugSaveConfigsInRootFolder  { get; set; }
        public bool PreventEmptyInputGetPathInArg0 { get; set; }
        public string ObjectsFilesFolder { get; set; }

        public static App Current { get; private set; }

        protected App() { }

        public static void Initialize()
        {
            App.Current = new App();
            App.Current.DebugShowArgsInput = true;
            App.Current.DebugShowExitConfirm = true;
            App.Current.DebugSaveConfigsInRootFolder  = true;
            App.Current.PreventEmptyInputGetPathInArg0 = true;
            App.Current.ObjectsFilesFolder = "Objects";
        }

        public virtual void Run()
        {            
            var args = Environment.GetCommandLineArgs();
            
#if DEBUG
            if (DebugShowArgsInput)
            {
                Console.WriteLine("Enter with args:");
                var read = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(read))
                    args = AppHelpers.CommandLineToArgs(read);
            }
#endif

            if (PreventEmptyInputGetPathInArg0 && args.Length == 1 && System.IO.File.Exists(args[0]))
                args = null;

            // retorna o nome do comando ou utiliza o padrão
            this.CurrentCommandName = App.COMMAND_NAME_DEFAULT;
            if (args != null && args.Length > 0 && args[0].Length > 0 && args[0][0] != '-')
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

        public virtual void SaveObjectFile<TOFile>(TOFile obj, string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = this.AutoGenerateObjectFileName(typeof(TOFile));

            ObjectFile<TOFile>.Save(obj, fileName);

            if (ObjectsFilesLoadeds.ContainsKey(fileName))
                ObjectsFilesLoadeds.Remove(fileName);
            //ObjectsFiles.Add(fileName, obj);
        }

        public virtual ObjectFile<TOFile> GetObjectFile<TOFile>(string fileName = null, bool refresh = false)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = this.AutoGenerateObjectFileName(typeof(TOFile));

            if (ObjectsFilesLoadeds.ContainsKey(fileName) && !refresh)
                return (ObjectFile<TOFile>)ObjectsFilesLoadeds[fileName];

            var objFile = ObjectFile<TOFile>.GetOrCreate(fileName);
            if (objFile != null)
                ObjectsFilesLoadeds[fileName] = objFile;

            return objFile;
        }

        public void IgnoreCommmand<T>()
        {
            IgnoredCommands.Add(typeof(T));
        }

        private string AutoGenerateObjectFileName(Type type)
        {
            string fileName;
            var attr = type.GetCustomAttributes(typeof(ObjectFileClassAttribute), true).FirstOrDefault() as ObjectFileClassAttribute;
            if (attr != null && !string.IsNullOrWhiteSpace(attr.FileName))
            {
                fileName = attr.FileName;
            }
            else
            {
                fileName = "syscmd." + AppHelpers.ToLowerSeparate(type.Name, ".") + ".object";
            }

            string folder = this.ObjectsFilesFolder;
            if (attr != null && !string.IsNullOrWhiteSpace(attr.Folder))
                folder = attr.Folder;

            return AppHelpers.GetPathFromRoot(folder, fileName);
        }
    }
}

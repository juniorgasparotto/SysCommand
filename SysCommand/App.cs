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
        public const string COMMAND_NAME_DEFAULT = "default";
        
        private List<Type> IgnoredCommands = new List<Type>();
        private Dictionary<string, object> ObjectsFiles = new Dictionary<string, object>();
        private Dictionary<string, object> ObjectsMemory = new Dictionary<string, object>();
        private List<ICommand> Commands { get; set; }

        public string CurrentCommandName { get; private set; }
        public bool InStopPropagation { get; private set; }

        public bool DebugShowArgsInput { get; set; }
        public bool DebugShowExitConfirm { get; set; }
        public bool DebugSaveConfigsInRootFolder  { get; set; }
        //public bool PreventEmptyInputGetPathInArg0 { get; set; }
        public string ObjectsFilesFolder { get; set; }
        public string ObjectsFilesPrefix { get; set; }
        public string ObjectsFilesExtension { get; set; }
        public bool ObjectsFilesUseTypeFullName { get; set; }
        public bool InDebug { get { return System.Diagnostics.Debugger.IsAttached; } }

        public static App Current { get; private set; }

        protected App() { }

        public static void Initialize()
        {
            Initialize<App>();
        }

        public static void Initialize<TApp>() where TApp : App
        {
            App.Current = Activator.CreateInstance<TApp>();
            App.Current.DebugShowArgsInput = true;
            App.Current.DebugShowExitConfirm = true;
            App.Current.DebugSaveConfigsInRootFolder  = true;
            //App.Current.PreventEmptyInputGetPathInArg0 = true;
            App.Current.ObjectsFilesFolder = ".app";
            App.Current.ObjectsFilesPrefix = ""; // "syscmd.";
            App.Current.ObjectsFilesExtension = ".object";
        }

        public virtual void Run()
        {            
            var args = Environment.GetCommandLineArgs();
            
            if (this.InDebug && DebugShowArgsInput)
            {
                Console.WriteLine("Enter with args:");
                var read = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(read))
                    args = AppHelpers.CommandLineToArgs(read);
            }
            
            //foreach (var i in args)
            //    Console.WriteLine(i);
            //return;

            //if (PreventEmptyInputGetPathInArg0 && args.Length == 1 && System.IO.File.Exists(args[0]))
            //    args = null;

            // ignore args0 because is exe path
            this.CurrentCommandName = App.COMMAND_NAME_DEFAULT;
            if (args != null && args.Length > 1 && args[1].Length > 0 && args[1][0] != '-')
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
                                      attr = assemblyType.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault() as CommandAttribute
                                  }).ToList();

            listOfCommands = listOfCommands.OrderBy(f => f.attr == null ? 0 : f.attr.OrderExecution).ToList();
            
            if (!this.InDebug)
                listOfCommands.RemoveAll(f => f.attr != null && f.attr.OnlyInDebug);

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
            if (this.InDebug)
            {
                Console.WriteLine("Press ENTER to Exit");
                Console.ReadLine();
            }
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

        public void IgnoreCommmand<T>()
        {
            IgnoredCommands.Add(typeof(T));
        }

        public virtual void SaveObjectMemory<TOMemory>(TOMemory obj, string name)
        {
            ObjectsMemory[name] = obj;
        }

        public virtual void RemoveObjectMemory<TOMemory>(string name) where TOMemory : class
        {
            if (this.ObjectsMemory.ContainsKey(name))
                this.ObjectsMemory.Remove(name);
        }

        public virtual TOMemory GetObjectMemory<TOMemory>(string name) where TOMemory : class
        {
            return GetOrCreateObjectMemory<TOMemory>(name, true);
        }

        public virtual TOMemory GetOrCreateObjectMemory<TOMemory>(string name, bool onlyGet = false) where TOMemory : class
        {
            TOMemory obj = null;
            if (this.ObjectsMemory.ContainsKey(name))
                obj = this.ObjectsMemory[name] as TOMemory;
            else if (!onlyGet)
                obj = Activator.CreateInstance<TOMemory>();
            return obj;
        }

        public virtual void SaveObjectFile<TOFile>(TOFile obj, string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = this.GetObjectFileName(typeof(TOFile), fileName);

            if (obj != null)
            {
                ObjectFile.Save<TOFile>(obj, fileName);
                this.ObjectsFiles[fileName] = obj;
            }
        }

        public virtual void RemoveObjectFile<TOFile>(string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = this.GetObjectFileName(typeof(TOFile), fileName);

            ObjectFile.Remove(fileName);
            if (this.ObjectsFiles.ContainsKey(fileName))
                this.ObjectsFiles.Remove(fileName);
        }

        public virtual TOFile GetObjectFile<TOFile>(string fileName = null, bool refresh = false) where TOFile : class
        {
            return GetOrCreateObjectFile<TOFile>(fileName, true, refresh);
        }

        public virtual TOFile GetOrCreateObjectFile<TOFile>(string fileName = null, bool onlyGet = false, bool refresh = false) where TOFile : class
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = this.GetObjectFileName(typeof(TOFile), fileName);

            if (this.ObjectsFiles.ContainsKey(fileName) && !refresh)
                return this.ObjectsFiles[fileName] as TOFile;

            var objFile = ObjectFile.Get<TOFile>(fileName);

            if (objFile == null && !onlyGet)
                objFile = Activator.CreateInstance<TOFile>();

            this.ObjectsFiles[fileName] = objFile;

            return objFile;
        }

        public string GetObjectFileName(Type type, string fileName = null, bool useTypeFullName = false)
        {
            string folder = null;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                var attr = type.GetCustomAttributes(typeof(ObjectFileAttribute), true).FirstOrDefault() as ObjectFileAttribute;
                if (attr != null && !string.IsNullOrWhiteSpace(attr.FileName))
                {
                    fileName = attr.FileName;
                }
                else
                {
                    useTypeFullName = !useTypeFullName ? this.ObjectsFilesUseTypeFullName : useTypeFullName;
                    fileName = AppHelpers.CSharpName(type, useTypeFullName).Replace("<", "[").Replace(">", "]").Replace(@"\", "");
                    fileName = App.Current.ObjectsFilesPrefix + AppHelpers.ToLowerSeparate(fileName, '.') + this.ObjectsFilesExtension;
                }

                folder = this.ObjectsFilesFolder;
                if (attr != null && !string.IsNullOrWhiteSpace(attr.Folder))
                    folder = attr.Folder;
            }

            if (string.IsNullOrWhiteSpace(folder))
                return AppHelpers.GetPathFromRoot(fileName);
            else
                return AppHelpers.GetPathFromRoot(folder, fileName);
        }
    }
}

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

        public VerboseEnum Verbose { get; set; }
        public bool Quiet { get; set; }

        public List<ICommand> Commands { get; set; }
        public List<CommandAction> Commands2 { get; set; }

        public string CurrentCommandName { get; set; }
        public char? ActionCharPrefix { get; set; }
        
        public bool InStopPropagation { get; private set; }

        public bool DebugGetInputArgs { get; set; }
        public bool DebugShowExitConfirm { get; set; }
        public bool DebugObjectsFilesSaveInRootFolder  { get; set; }

        public string ObjectsFilesFolder { get; set; }
        public string ObjectsFilesPrefix { get; set; }
        public string ObjectsFilesExtension { get; set; }
        public bool ObjectsFilesUseTypeFullName { get; set; }
        public bool InDebug { get { return System.Diagnostics.Debugger.IsAttached; } }
        public Request Request { get; private set; }
        public Response Response { get; private set; }

        public static App Current { get; private set; }

        protected App() { }

        public static void Initialize()
        {
            Initialize<App>();
        }

        public static void Initialize<TApp>() where TApp : App
        {
            App.Current = Activator.CreateInstance<TApp>();
            App.Current.DebugGetInputArgs = true;
            App.Current.DebugShowExitConfirm = true;
            App.Current.DebugObjectsFilesSaveInRootFolder  = true;
            App.Current.ObjectsFilesFolder = ".app";
            App.Current.ObjectsFilesPrefix = "";
            App.Current.ObjectsFilesExtension = ".object";
        }

        public virtual void Run()
        {            
            var args = Environment.GetCommandLineArgs();
            var listArgs = args.ToList();
            listArgs.RemoveAt(0);
            args = listArgs.ToArray();

            if (this.InDebug && DebugGetInputArgs)
            {
                Console.WriteLine("Enter with args:");
                var read = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(read))
                    args = AppHelpers.CommandLineToArgs(read);
            }

            this.Request = new Request(args);
            this.Response = new Response();


            var listOfCommands = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                         typeof(CommandAction).IsAssignableFrom(assemblyType)
                                      && assemblyType.IsInterface == false
                                      && assemblyType.IsAbstract == false
                                  select assemblyType).ToList();

            this.Commands2 = listOfCommands.Select(f => (CommandAction)Activator.CreateInstance(f)).OrderBy(f => f.OrderExecution).ToList();
            this.Commands2.RemoveAll(f => IgnoredCommands.Contains(f.GetType()) || (!this.InDebug && f.OnlyInDebug));

            foreach (var cmd in this.Commands2)
                cmd.Parse();

            var hasError = false;
            if (ValidateParserErrors2())
                this.ExecuteAll2();
            else
                hasError = true;

            this.LoadCommands();
            this.LoadAll(args);
            this.ParseAll();

            if (ValidateParserErrors())
                this.ExecuteAll();
            else
                hasError = true;
            
            if (hasError)
            {
                this.ShowHelp();
                this.ShowHelp2();
            }

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
                                  select assemblyType).ToList();

            this.Commands = listOfCommands.Select(f => (ICommand)Activator.CreateInstance(f)).OrderBy(f => f.OrderExecution).ToList();
            this.Commands.RemoveAll(f => IgnoredCommands.Contains(f.GetType()) || (!this.InDebug && f.OnlyInDebug));
        }

        protected virtual void LoadAll(string[] args)
        {
            foreach (var cmd in this.Commands)
                cmd.Load(args);
        }

        protected virtual void ParseAll()
        {
            foreach (var cmd in this.Commands)
                cmd.Parse();
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
            
            return !hasError;
        }

        protected virtual bool ValidateParserErrors2()
        {
            var hasError = false;

            foreach (var cmd in this.Commands2)
            {
                foreach (var action in cmd.Actions)
                {
                    if (action.Result != null && action.Result.HasErrors)
                    {
                        hasError = true;
                        Console.WriteLine(string.Format("Action '{0}({1} params)': {2}", action.Name, action.MethodInfo.GetParameters().Length, action.Result.ErrorText.Trim()));
                    }
                }
            }
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

        protected virtual void ExecuteAll2()
        {
            foreach (var cmd in this.Commands2)
            {
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

            if (dic.Count > 0)
                Console.WriteLine(AppHelpers.GetConsoleHelper(dic));
        }

        public virtual void ShowHelp2()
        {
            foreach (var cmd in this.Commands2)
            {
                foreach (var action in cmd.Actions)
                {
                    var dic = new Dictionary<string, string>();

                    Console.WriteLine("Action: {0}", this.ActionCharPrefix + action.Name);

                    foreach (var opt in action.Parser.Options)
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

                    if (dic.Count > 0)
                        Console.WriteLine(AppHelpers.GetConsoleHelper(dic));
                }
            }
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

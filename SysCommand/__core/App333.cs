using System.Collections.Generic;
using System.Linq;
using System;
using Fclp;
using System.Linq.Expressions;
using System.IO;

namespace SysCommand
{
    public class App333
    {
        public static readonly string CommandNameDefault = "default";
        
        private List<Type> ignoredCommands = new List<Type>();
        private Dictionary<string, object> objectsFiles = new Dictionary<string, object>();
        private Dictionary<string, object> objectsMemory = new Dictionary<string, object>();
        private List<ActionInstance> actions;
        private TextWriter output;
        private string[] args;

        public VerboseEnum Verbose { get; set; }
        public bool Quiet { get; set; }

        public List<ICommand> Commands2 { get; set; }
        public List<CommandAction> Commands { get; set; }
        public IEnumerable<ActionInstance> Actions
        {
            get
            {
                if (actions == null)
                    actions = Commands.SelectMany(f => f.Actions).ToList();
                return actions;
            }
        }

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

        public static App333 Current { get; private set; }

        protected App333() { }

        public static void Initialize()
        {
            Initialize<App333>();
        }

        public static void Initialize<TApp>() where TApp : App333
        {
            App333.Current = (TApp)Activator.CreateInstance(typeof(TApp), true);
            App333.Current.DebugGetInputArgs = true;
            App333.Current.DebugShowExitConfirm = true;
            App333.Current.DebugObjectsFilesSaveInRootFolder  = true;
            App333.Current.ObjectsFilesFolder = ".app";
            App333.Current.ObjectsFilesPrefix = "";
            App333.Current.ObjectsFilesExtension = ".object";
        }

        public void SetArgs(string[] args)
        {
            //this.output = output ?? Console.Out;
            this.args = args;
        }

        public void SetArgs(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
                this.args = AppHelpers.CommandLineToArgs(args);
            else
                this.args = new string[0];
        }

        private string[] GetOrCreateArgs()
        {
            if (this.args != null)
                return this.args;

            if (this.InDebug && DebugGetInputArgs)
            {
                Console.WriteLine("Enter with args:");
                this.SetArgs(Console.ReadLine());
            }
            else
            {
                this.args = Environment.GetCommandLineArgs();
                var listArgs = this.args.ToList();
                // remove the app path that added auto by .net
                listArgs.RemoveAt(0);
                this.SetArgs(listArgs.ToArray());
            }

            return this.args;
        }

        private void LoadRequestResponse()
        {
            var args = this.GetOrCreateArgs();
            this.Request = new Request(args);
            this.Response = new Response();
        }

        private void LoadCommandActions()
        {
            var listOfCommands = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from assemblyType in domainAssembly.GetTypes()
                                  where
                                         typeof(CommandAction).IsAssignableFrom(assemblyType)
                                      && assemblyType.IsInterface == false
                                      && assemblyType.IsAbstract == false
                                  select assemblyType).ToList();

            this.Commands = listOfCommands.Select(f => (CommandAction)Activator.CreateInstance(f)).OrderBy(f => f.OrderExecution).ToList();
            this.Commands.RemoveAll(f => ignoredCommands.Contains(f.GetType()) || (!this.InDebug && f.OnlyInDebug));
        }

        private void SetupCommandActions()
        {
            foreach (var cmd in this.Commands)
                cmd.Setup();
        }

        private void ParseCommandActions()
        {
            foreach (var cmd in this.Commands)
                cmd.Parse();
        }

        protected virtual bool ValidateCommandActions()
        {
            var hasError = false;

            foreach (var cmd in this.Commands)
            {
                foreach (var action in cmd.Actions)
                {
                    if (action.Result != null && action.Result.HasErrors)
                    {
                        hasError = true;
                        Console.WriteLine(string.Format("Action '{0}({1} args)': {2}", action.Name, action.MethodInfo.GetParameters().Length, action.Result.ErrorText.Trim()));
                    }
                }
            }
            return !hasError;
        }

        protected virtual void ExecuteCommandActions()
        {
            foreach (var cmd in this.Commands)
            {
                cmd.Execute();

                if (this.InStopPropagation)
                {
                    this.InStopPropagation = false;
                    break;
                }
            }
        }

        public virtual void Run()
        {
            this.Validate();
            this.LoadCommandActions();
            this.SetupCommandActions();
            this.LoadRequestResponse();
            this.ParseCommandActions();

            var hasError = false;
            if (this.ValidateCommandActions())
                this.ExecuteCommandActions();
            else
                hasError = true;
            return;
            this.LoadCommands();
            this.LoadAll(this.Request.Arguments);
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

            this.Commands2 = listOfCommands.Select(f => (ICommand)Activator.CreateInstance(f)).OrderBy(f => f.OrderExecution).ToList();
            this.Commands2.RemoveAll(f => ignoredCommands.Contains(f.GetType()) || (!this.InDebug && f.OnlyInDebug));
        }

        protected virtual void LoadAll(string[] args)
        {
            foreach (var cmd in this.Commands2)
                cmd.Load(args);
        }

        protected virtual void ParseAll()
        {
            foreach (var cmd in this.Commands2)
                cmd.Parse();
        }

        protected virtual bool ValidateParserErrors()
        {
            var hasError = false;

            foreach (var cmd in this.Commands2)
            {
                if (cmd.ParserResult.HasErrors)
                {
                    hasError = true;
                    Console.WriteLine(cmd.ParserResult.ErrorText);
                }
            }
            
            return !hasError;
        }


        public virtual void ShowHelp2()
        {
            foreach (var cmd in this.Commands)
            {
                foreach (var action in cmd.Actions)
                {
                    var dic = new Dictionary<string, string>();
                    var defaultHelp = action.IsDefault ? "[default], " : "";
                    Console.WriteLine(string.Format("    {0}{1}", defaultHelp, action.Name));

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
                        Console.WriteLine(AppHelpers.GetConsoleHelper(dic, 10));
                }
            }
        }

        protected virtual void ExecuteAll()
        {
            foreach (var cmd in this.Commands2)
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
            foreach (var cmd in this.Commands2)
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
                Console.WriteLine(AppHelpers.GetConsoleHelper(dic, 4));
        }

        public void IgnoreCommmand<T>()
        {
            ignoredCommands.Add(typeof(T));
        }

        public virtual void SaveObjectMemory<TMemory>(TMemory obj, string name)
        {
            objectsMemory[name] = obj;
        }

        public virtual void RemoveObjectMemory<TMemory>(string name) where TMemory : class
        {
            if (this.objectsMemory.ContainsKey(name))
                this.objectsMemory.Remove(name);
        }

        public virtual TMemory GetObjectMemory<TMemory>(string name) where TMemory : class
        {
            return GetOrCreateObjectMemory<TMemory>(name, true);
        }

        public virtual TMemory GetOrCreateObjectMemory<TMemory>(string name, bool onlyGet = false) where TMemory : class
        {
            TMemory obj = null;
            if (this.objectsMemory.ContainsKey(name))
                obj = this.objectsMemory[name] as TMemory;
            else if (!onlyGet)
                obj = Activator.CreateInstance<TMemory>();
            return obj;
        }

        public virtual void SaveObjectFile<TFile>(TFile obj, string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = this.GetObjectFileName(typeof(TFile), fileName);

            if (obj != null)
            {
                FileHelper.SaveObjectToFileJson(obj, fileName);
                this.objectsFiles[fileName] = obj;
            }
        }

        public virtual void RemoveObjectFile<TFile>(string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = this.GetObjectFileName(typeof(TFile), fileName);

            FileHelper.RemoveFile(fileName);
            if (this.objectsFiles.ContainsKey(fileName))
                this.objectsFiles.Remove(fileName);
        }

        public virtual TFile GetObjectFile<TFile>(string fileName = null, bool refresh = false) 
        {
            return GetOrCreateObjectFile<TFile>(fileName, true, refresh);
        }

        public virtual TFile GetOrCreateObjectFile<TFile>(string fileName = null, bool onlyGet = false, bool refresh = false) 
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = this.GetObjectFileName(typeof(TFile), fileName);

            if (this.objectsFiles.ContainsKey(fileName) && !refresh)
                return this.objectsFiles[fileName] == null ? default(TFile) : (TFile)this.objectsFiles[fileName];

            var objFile = FileHelper.GetObjectFromFileJson<TFile>(fileName);

            if (objFile == null && !onlyGet)
                objFile = Activator.CreateInstance<TFile>();

            this.objectsFiles[fileName] = objFile;

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
                    fileName = App333.Current.ObjectsFilesPrefix + AppHelpers.ToLowerSeparate(fileName, '.') + this.ObjectsFilesExtension;
                }

                folder = this.ObjectsFilesFolder;
                if (attr != null && !string.IsNullOrWhiteSpace(attr.Folder))
                    folder = attr.Folder;
            }

            if (string.IsNullOrWhiteSpace(folder))
                return this.GetPathFromRoot(fileName);
            else
                return this.GetPathFromRoot(folder, fileName);
        }

        public string GetPathFromRoot(params string[] paths)
        {
            if (this.InDebug && this.DebugObjectsFilesSaveInRootFolder)
            {
                var paths2 = paths.ToList();
                paths2.Insert(0, @"..\..\");
                return Path.Combine(paths2.ToArray());
            }

            return Path.Combine(paths);
        }

        private void Validate()
        {
            if (App333.Current.ActionCharPrefix != null && AppHelpers.IsArgumentFormat(App333.Current.ActionCharPrefix.ToString()))
                throw new Exception("Is not possible use the action prefix '" + App333.Current.ActionCharPrefix + "' because this char is used like of argument delimiter.");
        }
    }
}

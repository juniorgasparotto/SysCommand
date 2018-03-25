using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using SysCommand.Execution;
using SysCommand.Mapping;
using SysCommand.ConsoleApp.Commands;
using SysCommand.ConsoleApp.Helpers;
using SysCommand.ConsoleApp.Results;
using SysCommand.ConsoleApp.Handlers;
using SysCommand.ConsoleApp.Descriptor;
using SysCommand.ConsoleApp.Loader;
using SysCommand.Helpers;

//#if NETCORE1_6
//using SysCommand.Compatibility;
//#else
using System.Runtime.Serialization;
using System.IO;
//#endif

namespace SysCommand.ConsoleApp
{
    public class App
    {
        /// <summary>
        /// Fires at the end of the implementation
        /// </summary>
        public Action<ApplicationResult> OnComplete { get; set; }

        /// <summary>
        /// Fires in case of exception.
        /// </summary>
        public Action<ApplicationResult, Exception> OnException { get; set; }

        /// <summary>
        /// Fires before invoking each Member (property or method) that was parsed.
        /// </summary>
        public Action<ApplicationResult, IMemberResult> OnBeforeMemberInvoke { get; set; }

        /// <summary>
        /// Fires after invoking each Member (property or method) that was parsed.
        /// </summary>
        public Action<ApplicationResult, IMemberResult> OnAfterMemberInvoke { get; set; }

        /// <summary>
        /// Fires when a method returns value
        /// </summary>
        public Action<ApplicationResult, IMemberResult> OnMethodReturn { get; set; }

        private readonly bool _enableMultiAction;
        private readonly IExecutor _executor;

        private IDescriptor _descriptor;
        private ConsoleWrapper _console;
        private TextWriter _output;
        private ItemCollection _items;

        /// <summary>
        /// Get all command maps
        /// </summary>
        public IEnumerable<CommandMap> Maps { get; private set; }

        /// <summary>
        /// Get all commands
        /// </summary>
        public IEnumerable<Command> Commands { get; private set; }

        /// <summary>
        /// Set ou Get the console wrapper
        /// </summary>
        public ConsoleWrapper Console
        {
            get
            {
                return this._console ?? (this._console = new ConsoleWrapper(_output));
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("This property can't be null.");
                
                this._console = value;
            }
        }

        /// <summary>
        /// Set or Get the descriptor.
        /// </summary>
        public IDescriptor Descriptor
        {
            get
            {
                return _descriptor ?? (_descriptor = new DefaultDescriptor());
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("This property can't be null.");

                _descriptor = value;
            }
        }

        /// <summary>
        /// Scope of variables for this App
        /// </summary>
        public ItemCollection Items
        {
            get
            {
                return this._items ?? (this._items = new ItemCollection());
            }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="commandsTypes">Specifies the Command types that will be used throughout the process. If it is null then the system will try to automatically any class that extend from Command</param>
        /// <param name="enableMultiAction">Turns the MultiAction behavior. By default, this behavior will be enabled</param>
        /// <param name="addDefaultAppHandler">If false so does not create the event handler that is responsible for the outputs standard engine and erros controls, and among others. The default is true</param>
        public App(
            IEnumerable<Type> commandsTypes = null,
            bool enableMultiAction = true,
            bool addDefaultAppHandler = true,
            TextWriter output = null
        )
        {
            this._enableMultiAction = enableMultiAction;
            this._output = output;

            // default executor
            this._executor = new DefaultExecutor.Executor();

            // add handler default
            if (addDefaultAppHandler)
                this.AddApplicationHandler(new DefaultApplicationHandler());

            // init commands
            this.Initialize(commandsTypes);            
        }

        private void Initialize(IEnumerable<Type> commandsTypes = null)
        {
            // load all in app domain if the list = null
            if (commandsTypes == null)
                commandsTypes = new AppDomainCommandLoader().GetFromAppDomain();

            var propAppName = typeof(Command).GetProperties().First(p => p.PropertyType == typeof(App)).Name;
            var commands = commandsTypes
                .Select(type => this.CreateCommandInstance(type, propAppName))
                .ToList();

            // remove commands that are only for debugs
            commands.RemoveAll(f => !Development.IsAttached && f.OnlyInDebug);
            
            // validate if the list is empty
            if (!commands.Any())
                throw new Exception("No command found");

            var helpCommands = commands.Where(f => f is IHelpCommand).ToList();
            if (helpCommands.Empty())
            {
                commands = new List<Command>(commands)
                {
                    this.CreateCommandInstance<HelpCommand>(propAppName)
                };
            }
            else if (helpCommands.Count > 1)
            {
                commands.Remove(helpCommands.First(f=> f is HelpCommand));
            }

            // mapping
            this.Maps = this._executor.GetMaps(commands).ToList();
            this.Commands = commands;
        }

        /// <summary>
        /// Add a new handler to intercept all events
        /// </summary>
        /// <param name="handler">Instance of handler</param>
        /// <returns>The same App instance</returns>
        public App AddApplicationHandler(IApplicationHandler handler)
        {
            this.OnComplete += handler.OnComplete;
            this.OnException += handler.OnException;
            this.OnAfterMemberInvoke += handler.OnAfterMemberInvoke;
            this.OnBeforeMemberInvoke += handler.OnBeforeMemberInvoke;
            this.OnMethodReturn += handler.OnMethodReturn;
            return this;
        }

        /// <summary>
        /// Run the application
        /// </summary>
        /// <param name="arg">Command line as string</param>
        /// <returns>Return the application result</returns>
        public ApplicationResult Run(string arg)
        {
            return this.Run(ConsoleAppHelper.StringToArgs(arg));
        }

        /// <summary>
        /// Run the application
        /// </summary>
        /// <param name="args">Command line as array of string (arguments)</param>
        /// <returns>Return the application result</returns>
        public ApplicationResult Run(string[] args)
        {
            var appResult = new ApplicationResult
            {
                App = this,
                Args = args,
                ArgsOriginal = args
            };

            Start:

            try
            {
                var userMaps = this.Maps.ToList();
                var executed = false;
                var isRestarted = false;

                appResult.ArgumentsRaw = this._executor.ParseRaw(appResult.Args, this.Maps);
                Action<IMemberResult, Execution.ExecutionScope> invokeAction;
                invokeAction = (member, scope) =>
                {
                    var actionResult = this.InvokeMemberInternal(appResult, member);

                    if (actionResult != null)
                    {
                        if (actionResult is RedirectResult)
                        {
                            isRestarted = true;
                            appResult.Args = ((RedirectResult)actionResult).NewArgs;
                            scope.StopPropagation();
                        }
                        else
                        {
                            if (this.OnMethodReturn != null)
                                this.OnMethodReturn(appResult, member);
                        }
                    }
                };

                // system feature: "help"
                var helpCommand = this.Maps.GetCommandMap<IHelpCommand>();
                if (helpCommand != null)
                {
                    var executorHelp = new DefaultExecutor.Executor();
                    var maps = new List<CommandMap> { helpCommand };
                    var parseResultHelp = executorHelp.Parse(appResult.Args, appResult.ArgumentsRaw, maps, false);
                    var executionResult = executorHelp.Execute(parseResultHelp, invokeAction);
                    if (executionResult.State == ExecutionState.Success)
                    {
                        appResult.ParseResult = parseResultHelp;
                        appResult.ExecutionResult = executionResult;
                        executed = true;
                    }
                    userMaps.Remove(helpCommand);
                }

                if (!executed)
                {
                    appResult.ParseResult = this._executor.Parse(appResult.Args, appResult.ArgumentsRaw, userMaps, this._enableMultiAction);
                    appResult.ExecutionResult = this._executor.Execute(appResult.ParseResult, invokeAction);
                }

                if (isRestarted)
                    goto Start;

                if (this.OnComplete != null)
                    this.OnComplete(appResult);
            }
            catch(Exception ex)
            {
                if (this.OnException != null)
                    this.OnException(appResult, ex);
                else
                    throw ex;
            }

            return appResult;
        }

        private IActionResult InvokeMemberInternal(ApplicationResult args, IMemberResult member)
        {
            if (!member.IsInvoked)
            {
                if (this.OnBeforeMemberInvoke != null)
                    this.OnBeforeMemberInvoke(args, member);
                
                if (!member.IsInvoked)
                {
                    member.Invoke();
                    if (this.OnAfterMemberInvoke != null)
                        this.OnAfterMemberInvoke(args, member);
                }
            }

            MethodInfo method = null;
            if (member is MethodResult)
                method = ((MethodResult)member).MethodInfo;
            else if (member is MethodMainResult)
                method = ((MethodMainResult)member).MethodInfo;

            if (method == null || method.ReturnType == typeof(void) || member.Value == null)
                return null;

            var value = member.Value as IActionResult;
            return value ?? new ActionResult(member.Value);
        }
        
        private Command CreateCommandInstance<T>(string propertyAppName)
        {
            return this.CreateCommandInstance(typeof(T), propertyAppName);
        }

        private Command CreateCommandInstance(Type type, string propertyAppName)
        {
            var obj = FormatterServices.GetUninitializedObject(type);

            obj.GetType().GetProperty(propertyAppName).SetValue(obj, this);
            obj.GetType().GetConstructor(Type.EmptyTypes)?.Invoke(obj, null);
            return (Command) obj;
        }

        /// <summary>
        /// Run application with console simulator. This feature helps you test your inputs inside 
        /// the Visual Studio without having to run your ".exe" in an external console. 
        /// It is important to note that this Simulator will only be displayed within Visual Studio.
        /// </summary>
        /// <param name="appFactory"></param>
        /// <param name="breakEndLine"></param>
        /// <returns></returns>
        public static int RunApplication(Func<App> appFactory = null, bool breakEndLine = true)
        {
            var lastBreakLineInNextWrite = false;
            while (true)
            {
                var app = appFactory != null ? appFactory() : new App();
                app.Console.BreakLineInNextWrite = lastBreakLineInNextWrite;
                app.Run(GetArguments(app));
                lastBreakLineInNextWrite = app.Console.BreakLineInNextWrite;

                if (!Development.IsAttached)
                {
                    if (breakEndLine)
                        app.Console.Write("\r\n");

                    return app.Console.ExitCode;
                }
            }
        }
        
        private static string[] GetArguments(App app)
        {
            if (Development.IsAttached)
            {
                var args = app.Console.Read(Strings.CmdIndicator);
                return ConsoleAppHelper.StringToArgs(args);
            }

            var listArgs = Environment.GetCommandLineArgs().ToList();

            // remove the app path that added auto by .net
            listArgs.RemoveAt(0);

            return listArgs.ToArray();
        }
    }
}

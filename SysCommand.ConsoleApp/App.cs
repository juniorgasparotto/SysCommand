using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using SysCommand.Execution;
using SysCommand.Mapping;
using SysCommand.ConsoleApp.Commands;
using SysCommand.ConsoleApp.Helpers;
using System.Runtime.Serialization;
using SysCommand.ConsoleApp.Results;
using SysCommand.ConsoleApp.Handlers;
using SysCommand.ConsoleApp.Descriptor;
using SysCommand.ConsoleApp.Loader;

namespace SysCommand.ConsoleApp
{
    public class App
    {
        public event AppRunCompleteHandler OnComplete;
        public event AppRunExceptionHandler OnException;
        public event AppRunOnBeforeMemberInvokeHandler OnBeforeMemberInvoke;
        public event AppRunOnAfterMemberInvokeHandler OnAfterMemberInvoke;
        public event AppRunOnMethodReturnHandler OnMethodReturn;

        private bool enableMultiAction;
        private IExecutor executor;
        private IDescriptor descriptor;
        private ConsoleWrapper console;
        private ItemCollection items;

        public bool ReadArgsWhenIsDebug { get; set; }
        public IEnumerable<CommandMap> Maps { get; private set; }
        public IEnumerable<Command> Commands { get; private set; }

        public ConsoleWrapper Console
        {
            get
            {
                if (this.console == null)
                    this.console = new ConsoleWrapper();
                return this.console;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("This property can't be null.");

                this.console = value;
            }
        }

        public IDescriptor Descriptor
        {
            get
            {
                if (descriptor == null)
                    descriptor = new DefaultDescriptor();
                return descriptor;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("This property can't be null.");

                descriptor = value;
            }
        }

        public ItemCollection Items
        {
            get
            {
                if (this.items == null)
                    this.items = new ItemCollection();

                return this.items;
            }
        }
        
        public App(
            IEnumerable<Type> commandsTypes = null,
            bool enableMultiAction = true,
            IExecutor executor = null,
            bool addDefaultAppHandler = true
        )
        {
            this.enableMultiAction = enableMultiAction;

            // default executor
            this.executor = executor ?? new DefaultExecutor.Executor();

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
                commandsTypes = new AppDomainCommandLoader().GetFromAppDomain(DebugHelper.IsDebug);

            var propAppName = typeof(Command).GetProperties().First(p => p.PropertyType == typeof(App)).Name;
            var commands = commandsTypes
                .Select(type => this.CreateCommandInstance(type, propAppName))
                .ToList();

            // remove commands that are only for debugs
            commands.RemoveAll(f => !DebugHelper.IsDebug && f.OnlyInDebug);
            
            // validate if the list is empty
            if (!commands.Any())
                throw new Exception("No command found");

            if (!commands.Any(f => f is IHelpCommand))
                commands = new List<Command>(commands)
                    {
                        this.CreateCommandInstance<HelpCommand>(propAppName)
                    };

            // mapping
            this.Maps = this.executor.GetMaps(commands).ToList();
            this.Commands = commands;
        }

        public App AddApplicationHandler(IApplicationHandler handler)
        {
            this.OnComplete += handler.OnComplete;
            this.OnException += handler.OnException;
            this.OnAfterMemberInvoke += handler.OnAfterMemberInvoke;
            this.OnBeforeMemberInvoke += handler.OnBeforeMemberInvoke;
            this.OnMethodReturn += handler.OnMethodReturn;
            return this;
        }

        public ApplicationResult Run()
        {
            return this.Run(GetArguments());
        }

        public ApplicationResult Run(string arg)
        {
            return this.Run(ConsoleAppHelper.StringToArgs(arg));
        }

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

                appResult.ArgumentsRaw = this.executor.ParseRaw(appResult.Args, this.Maps);

                // system feature: "manage args history"
                //var manageCommand = this.Maps.GetMap<IManageArgsHistoryCommand>();
                //if (manageCommand != null)
                //{
                //    var executorHistory = new DefaultExecutor.Executor();
                //    var maps = new List<CommandMap> { manageCommand };
                //    var parseResultHistory = executorHistory.Parse(appResult.Args, appResult.ArgumentsRaw, maps, false);
                //    var newArgs = executorHistory.Execute(parseResultHistory, null).Results.GetValue<string[]>();
                //    if (newArgs != null)
                //        appResult.Args = newArgs;

                //    userMaps.Remove(manageCommand);
                //}

                Action<IMemberResult, ExecutionScope> invokeAction;
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
                var helpCommand = this.Maps.GetMap<IHelpCommand>();
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
                    appResult.ParseResult = this.executor.Parse(appResult.Args, appResult.ArgumentsRaw, userMaps, this.enableMultiAction);
                    appResult.ExecutionResult = this.executor.Execute(appResult.ParseResult, invokeAction);
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
        
        private string[] GetArguments()
        {
            if (DebugHelper.IsDebug && this.ReadArgsWhenIsDebug)
            {
                var args = this.Console.Read(Strings.GetArgumentsInDebug);
                return ConsoleAppHelper.StringToArgs(args);
            }
            else
            {
                var listArgs = Environment.GetCommandLineArgs().ToList();
                // remove the app path that added auto by .net
                listArgs.RemoveAt(0);
                return listArgs.ToArray();
            }
        }

        private Command CreateCommandInstance<T>(string propertyAppName)
        {
            return this.CreateCommandInstance(typeof(T), propertyAppName);
        }

        private Command CreateCommandInstance(Type type, string propertyAppName)
        {
            object obj = FormatterServices.GetUninitializedObject(type);
            obj.GetType().GetProperty(propertyAppName).SetValue(obj, this);
            obj.GetType().GetConstructor(Type.EmptyTypes).Invoke(obj, null);
            return (Command)obj;
        }

        public static int RunApplication(Func<App> appFactory = null)
        {
            var lastBreakLineInNextWrite = false;
            while (true)
            {
                var app = appFactory != null ? appFactory() : new App();
                app.ReadArgsWhenIsDebug = true;
                app.Console.BreakLineInNextWrite = lastBreakLineInNextWrite;
                app.Run();
                lastBreakLineInNextWrite = app.Console.BreakLineInNextWrite;

                if (!DebugHelper.IsDebug)
                    return app.Console.ExitCode;
            }
        }

    }
}

﻿using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using SysCommand.Execution;
using SysCommand.Mapping;
using SysCommand.ConsoleApp.Commands;

namespace SysCommand.ConsoleApp
{
    public class App
    {
        public static bool IsDebug { get { return System.Diagnostics.Debugger.IsAttached; } }

        public event AppRunCompleteHandler OnComplete;
        public event AppRunExceptionHandler OnException;
        public event AppRunOnBeforeMemberInvokeHandler OnBeforeMemberInvoke;
        public event AppRunOnAfterMemberInvokeHandler OnAfterMemberInvoke;
        public event AppRunOnMethodReturnHandler OnMethodReturn;

        private bool enableMultiAction;
        private IExecutor executor;
        private IDescriptor descriptor;
        private ConsoleWrapper console;

        public bool ReadArgsWhenIsDebug { get; set; }
        public IEnumerable<CommandMap> Maps { get; private set; }

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

        public App(
            IEnumerable<Command> commands = null,
            IExecutor executor = null,
            bool enableMultiAction = true,
            bool addDefaultAppHandler = true
        )
        {
            this.enableMultiAction = enableMultiAction;

            // validate if some commands is attached in another app.
            if (commands != null)
            {
                foreach (var command in commands)
                    if (command.App != null)
                        throw new Exception(string.Format("The command '{0}' already attached to another application.", command.GetType().FullName));
            }
            
            // load all in app domain if the list = null
            commands = commands ?? new AppDomainCommandLoader().GetFromAppDomain(IsDebug);

            // validate if the list is empty
            if (!commands.Any())
                throw new Exception("No command found");

            if (!commands.Any(f => f is IHelpCommand))
                commands = new List<Command>(commands) { new HelpCommand() };

            foreach (var command in commands)
                command.App = this;

            // default executor
            this.executor = executor ?? new DefaultExecutor.Executor();

            // add handler default
            if (addDefaultAppHandler)
                this.AddApplicationHandler(new DefaultApplicationHandler());

            // mapping
            this.Maps = this.executor.GetMaps(commands).ToList();
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
            var appResult = new ApplicationResult();
            appResult.App = this;
            appResult.Args = args;
            appResult.ArgsOriginal = args;

            try
            {
                var userMaps = this.Maps.ToList();
                var executed = false;

                // system feature: "manage args history"
                var manageCommand = this.Maps.GetMap<IManageArgsHistoryCommand>();
                if (manageCommand != null)
                {
                    //var executorHistory = new DefaultExecutor.Executor();
                    //var parseResultHistory = executorHistory.Parse(appResult.Args, new List<CommandMap> { manageCommand }, false);
                    //var newArgs = executorHistory.Execute(parseResultHistory, null).Results.GetValue<string[]>();
                    //if (newArgs != null)
                    //    appResult.Args = newArgs;

                    userMaps.Remove(manageCommand);
                }

                // system feature: "help"
                var helpCommand = this.Maps.GetMap<IHelpCommand>();
                if (helpCommand != null)
                {
                    var executorHelp = new DefaultExecutor.Executor();
                    var parseResultHelp = executorHelp.Parse(appResult.Args, new List<CommandMap> { helpCommand }, false);
                    var executionResult = executorHelp.Execute(parseResultHelp, (member) => this.MemberInvoke(appResult, member));
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
                    appResult.ParseResult = this.executor.Parse(appResult.Args, userMaps, this.enableMultiAction);
                    appResult.ExecutionResult = this.executor.Execute(appResult.ParseResult, (member) => this.MemberInvoke(appResult, member));
                }

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

        private void MemberInvoke(ApplicationResult args, IMemberResult member)
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

            if (method != null && method.ReturnType != typeof(void) && member.Value != null)
            {
                if (this.OnMethodReturn != null)
                    this.OnMethodReturn(args, member);
            }
        }

        private string[] GetArguments()
        {
            if (App.IsDebug && this.ReadArgsWhenIsDebug)
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

        public static int RunInfiniteIfDebug(App app = null)
        {
            app = app ?? new App();
            bool lastBreakLineInNextWrite = false;
            while (true)
            {
                app.ReadArgsWhenIsDebug = true;
                app.Console.BreakLineInNextWrite = lastBreakLineInNextWrite;
                app.Run();
                lastBreakLineInNextWrite = app.Console.BreakLineInNextWrite;

                if (!App.IsDebug)
                    return app.Console.ExitCode;
            }
        }

    }
}

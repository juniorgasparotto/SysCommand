using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Reflection;

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
        private IMappingStrategy mapper;
        private IEvaluationStrategy evaluationStrategy;
        private IMessageFormatter messageFormatter;
        private ConsoleWrapper console;

        public bool ReadArgsWhenIsDebug { get; set; }
        public IEnumerable<CommandMap> Maps { get; private set; }

        public ConsoleWrapper Console
        {
            get
            {
                return console ?? new ConsoleWrapper();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("This property can't be null.");

                console = value;
            }
        }

        public IMessageFormatter MessageFormatter
        {
            get
            {
                return messageFormatter ?? new DefaultMessageFormatter();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("This property can't be null.");

                messageFormatter = value;
            }
        }

        public App(
            IEnumerable<Command> commands = null,
            bool enableMultiAction = true,
            IMappingStrategy mapper = null,
            IEvaluationStrategy evaluationStrategy = null,
            bool addDefaultAppHandler = true
        )
        {
            // validate if some commands is attached in another app.
            if (commands != null)
            {
                foreach (var command in commands)
                    if (command.App != null)
                        throw new Exception(string.Format("The command '{0}' already attached to another application.", command.GetType().FullName));
            }
            
            // load all in app domain if the list = null
            commands = commands ?? new AppDomainCommandLoader().GetFromAppDomain(IsDebug);
            foreach (var command in commands)
                command.App = this;

            // validate if the list is empty
            if (commands.Empty())
                throw new Exception("No command found");

            this.enableMultiAction = enableMultiAction;

            // defaults
            this.Console = new ConsoleWrapper();
            this.mapper = mapper ?? new DefaultMappingStrategy();
            this.evaluationStrategy = evaluationStrategy ?? new DefaultEvaluationStrategy();

            // add handler default
            if (addDefaultAppHandler)
                this.AddApplicationHandler(new DefaultApplicationHandler());

            // mapping
            this.Maps = this.mapper.DoMappping(commands).ToList();
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
            return this.Run(AppHelpers.StringToArgs(arg));
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

                // system feature: "manage args history"
                var manageCommand = this.Maps.GetMap<IManageArgsHistoryCommand>();
                if (manageCommand != null)
                {
                    var evaluator = new Evaluator(appResult.Args, manageCommand, false, this.evaluationStrategy);
                    //this.Result.AddRange(evaluator.Eval().Result);
                    var newArgs = evaluator.Evaluate().Result.GetValue<string[]>();
                    if (newArgs != null)
                        appResult.Args = newArgs;

                    userMaps.Remove(manageCommand);
                }

                // system feature: "help"
                //var helpCommand = this.Maps.GetMap<IHelpCommand>();
                //if (helpCommand != null)
                //{

                //    var evaluator = new Evaluator(this.Args, helpCommand, false, this.evaluationStrategy);
                //    evaluator.Eval();

                //    if (evaluator.EvalState == EvalState.Success)
                //    { 
                //        this.Result.AddRange(evaluator.Result);
                //        return this;
                //    }

                //    userMaps.Remove(helpCommand);
                //}

                // execute user properties and methods
                var evaluator2 = new Evaluator(appResult.Args, userMaps, this.enableMultiAction, this.evaluationStrategy)
                    .OnInvoke(member => this.MemberInvoke(appResult, member));

                appResult.ParseResult = evaluator2.ParseResult;
                appResult.EvaluateResult = evaluator2.Evaluate();

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

        private void MemberInvoke(ApplicationResult args, IMember member)
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
            if (member is Method)
                method = ((Method)member).MethodInfo;
            else if (member is MethodMain)
                method = ((MethodMain)member).MethodInfo;

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
                return AppHelpers.StringToArgs(args);
            }
            else
            {
                var listArgs = Environment.GetCommandLineArgs().ToList();
                // remove the app path that added auto by .net
                listArgs.RemoveAt(0);
                return listArgs.ToArray();
            }
        }

    }
}

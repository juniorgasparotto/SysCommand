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

        private AppRunCompleteHandler onComplete;
        private AppRunExceptionHandler onException;
        private AppRunOnBeforeMemberInvokeHandler onBeforeMemberInvoke;
        private AppRunOnAfterMemberInvokeHandler onAfterMemberInvoke;
        private AppRunOnMethodReturnHandler onMethodReturn;

        private bool enableMultiAction;
        private IMapper mapper;
        private IEvaluationStrategy evaluationStrategy;
        private IMessageOutput messageOutput;

        public bool ReadArgsWhenIsDebug { get; set; }
        public IEnumerable<CommandMap> Maps { get; private set; }

        public ConsoleWrapper Console { get; set; }
        public IMessageOutput MessageOutput
        {
            get
            {
                return messageOutput ?? new MessageOutput();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("This property can't be null.");

                messageOutput = value;
            }
        }

        public App(
            IEnumerable<Command> commands = null,
            bool enableMultiAction = true,
            IMapper mapper = null,
            IEvaluationStrategy evaluationStrategy = null,
            IEventListener listener = null
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
            this.mapper = mapper ?? new DefaultMapper();
            this.evaluationStrategy = evaluationStrategy ?? new DefaultEvaluationStrategy();

            // events
            listener = listener ?? new DefaultEventListener();
            this.onComplete = listener.OnComplete;
            this.onException = listener.OnException;
            this.onAfterMemberInvoke = listener.OnAfterMemberInvoke;
            this.onBeforeMemberInvoke = listener.OnBeforeMemberInvoke;
            this.onMethodReturn = listener.OnMethodReturn;

            // mapping
            this.Maps = this.mapper.CreateMap(commands).ToList();
        }

        public App OnComplete(AppRunCompleteHandler onComplete)
        {
            this.onComplete = onComplete;
            return this;
        }

        public App OnException(AppRunExceptionHandler onException)
        {
            this.onException = onException;
            return this;
        }

        public App OnBeforeMemberInvoke(AppRunOnBeforeMemberInvokeHandler onBefore)
        {
            this.onBeforeMemberInvoke = onBefore;
            return this;
        }

        public App OnAfterMemberInvoke(AppRunOnAfterMemberInvokeHandler onAfter)
        {
            this.onAfterMemberInvoke = onAfter;
            return this;
        }

        public App OnMethotReturn(AppRunOnMethodReturnHandler onMethodReturn)
        {
            this.onMethodReturn = onMethodReturn;
            return this;
        }

        public AppResult Run()
        {
            return this.Run(GetArguments());
        }

        public AppResult Run(string arg)
        {
            return this.Run(AppHelpers.StringToArgs(arg));
        }

        public AppResult Run(string[] args)
        {
            var appResult = new AppResult();
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

                if (this.onComplete != null)
                    this.onComplete(appResult);
            }
            catch(Exception ex)
            {
                if (this.onException != null)
                    this.onException(appResult, ex);
                else
                    throw ex;
            }

            return appResult;
        }

        private void MemberInvoke(AppResult args, IMember member)
        {
            if (!member.IsInvoked)
            {
                if (this.onBeforeMemberInvoke != null)
                    this.onBeforeMemberInvoke(args, member);

                if (!member.IsInvoked)
                {
                    member.Invoke();
                    if (this.onAfterMemberInvoke != null)
                        this.onAfterMemberInvoke(args, member);
                }
            }

            MethodInfo method = null;
            if (member is Method)
                method = ((Method)member).MethodInfo;
            else if (member is MethodMain)
                method = ((MethodMain)member).MethodInfo;

            if (method != null && method.ReturnType != typeof(void) && member.Value != null)
            {
                this.onMethodReturn(args, member);
            }
        }

        private string[] GetArguments()
        {
            if (App.IsDebug && this.ReadArgsWhenIsDebug)
            {
                var args = this.Console.Read("Enter with args: ");
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

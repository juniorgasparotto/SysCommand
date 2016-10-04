using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace SysCommand.ConsoleApp
{
    public class App
    {
        public static bool IsDebug { get { return System.Diagnostics.Debugger.IsAttached; } }
        public static bool ReadArgsWhenIsDebug { get; set; }

        public delegate void AppRunExceptionArgs(App app, Exception ex);
        public delegate void AppRunCompleteArgs(App app, EvalState state);

        private AppRunCompleteArgs onCompleteCallBack;
        private AppRunExceptionArgs onExceptionCallBack;

        private bool enableMultiAction;
        private IMapper mapper;
        private IEvaluationStrategy evaluationStrategy;
        //private IListener listener;

        public string[] Args { get; private set; }
        public string[] ArgsOriginal { get; private set; }
        public Result<IMember> Result { get; private set; }
        public IEnumerable<CommandMap> Maps { get; private set; }
        public TextWriter Output { get; private set; }

        public App(
            string args,
            Command command,
            bool enableMultiAction = true,
            TextWriter output = null,
            IMapper mapper = null,
            IEvaluationStrategy evaluationStrategy = null,
            IEventListener listener = null
        ) : this(args, new List<Command> { command }, enableMultiAction, output, mapper, evaluationStrategy, listener)
        {

        }

        public App(
            string[] args,
            Command command,
            bool enableMultiAction = true,
            TextWriter output = null,
            IMapper mapper = null,
            IEvaluationStrategy evaluationStrategy = null,
            IEventListener listener = null
        ) : this(args, new List<Command> { command }, enableMultiAction, output, mapper, evaluationStrategy, listener)
        {

        }

        public App(
            string args,
            IEnumerable<Command> commands,
            bool enableMultiAction = true,
            TextWriter output = null,
            IMapper mapper = null,
            IEvaluationStrategy evaluationStrategy = null,
            IEventListener listener = null
        ) : this(AppHelpers.StringToArgs(args), commands, enableMultiAction, output, mapper, evaluationStrategy, listener)
        {

        }

        public App(
            string[] args,
            IEnumerable<Command> commands,
            bool enableMultiAction = true,
            TextWriter output = null,
            IMapper mapper = null,
            IEvaluationStrategy evaluationStrategy = null,
            IEventListener listener = null
        )
        {
            // validate if some commands is attached in another app.
            if (commands != null)
            {
                foreach (var command in commands)
                {
                    if (command.App == null)
                        command.App = this;
                    else
                        throw new Exception(string.Format("The '{0}' command already attached to another application.", command.GetType().FullName));
                }
            }
            
            // load all in app domain if the list = null
            commands = commands ?? new AppDomainCommandLoader().GetFromAppDomain(IsDebug);
            
            // validate if the list is empty
            if (commands.Empty())
                throw new Exception("No command found");

            this.Output = output ?? Console.Out;
            this.Args = GetArguments(args);
            this.ArgsOriginal = this.Args;
            this.enableMultiAction = enableMultiAction;
            this.mapper = mapper ?? new DefaultMapper();
            this.evaluationStrategy = evaluationStrategy ?? new DefaultEvaluationStrategy();
            this.Maps = this.mapper.CreateMap(commands);

            listener = listener ?? new DefaultEventListener();
            this.onCompleteCallBack = listener.OnComplete;
            this.onExceptionCallBack = listener.OnException;
        }

        public App OnComplete(AppRunCompleteArgs onComplete)
        {
            this.onCompleteCallBack = onComplete;
            return this;
        }

        public App OnException(AppRunExceptionArgs onException)
        {
            this.onExceptionCallBack = onException;
            return this;
        }

        public Result<IMember> Run()
        {
            try
            {
                var newMaps = this.Maps.ToList();

                // system feature: "manage args history"
                var manageCommand = this.Maps.GetMap<IManageArgsHistoryCommand>();
                if (manageCommand != null)
                {
                    var newArgs = new Evaluator(this.Args, manageCommand, false, this.evaluationStrategy).Eval().GetValue<string[]>();
                    if (newArgs != null)
                        this.Args = newArgs;

                    newMaps.Remove(manageCommand);
                }

                // system feature: "help"
                var helpCommand = this.Maps.GetMap<IHelpCommand>();
                if (helpCommand != null)
                {

                    var evaluator = new Evaluator(this.Args, helpCommand, false, this.evaluationStrategy);
                    evaluator.Eval();
                    if (evaluator.EvalState == EvalState.Success)
                        return evaluator.Result;

                    newMaps.Remove(helpCommand);
                }

                // execute user properties and methods
                var evaluatorUser = new Evaluator(this.Args, newMaps, this.enableMultiAction, this.evaluationStrategy);
                this.Result = evaluatorUser.Eval();

                if (this.onCompleteCallBack != null)
                    this.onCompleteCallBack(this, evaluatorUser.EvalState);

                return this.Result;
            }
            catch(Exception ex)
            {
                if (this.onExceptionCallBack != null)
                    this.onExceptionCallBack(this, ex);
                throw ex;
            }
        }

        public static string[] GetArguments(string[] args = null)
        {
            if (App.IsDebug && App.ReadArgsWhenIsDebug)
            {
                Console.WriteLine("Enter with args:");
                return AppHelpers.StringToArgs(Console.ReadLine());
            }
            else if (args == null)
            {
                var listArgs = Environment.GetCommandLineArgs().ToList();
                // remove the app path that added auto by .net
                listArgs.RemoveAt(0);
                return listArgs.ToArray();
            }

            return args;
        }
    }
}

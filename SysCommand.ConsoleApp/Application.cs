using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace SysCommand.ConsoleApp
{
    public class Application
    {
        public static bool IsDebug { get { return System.Diagnostics.Debugger.IsAttached; } }
        public static bool ReadArgsWhenIsDebug { get; set; }

        private bool enableMultiAction;
        private IMapper mapper;
        private IExecutionStrategy executionStrategy;
        private IExecutionListener executionListener;

        public string[] Args { get; private set; }
        public string[] ArgsOriginal { get; private set; }
        public IEnumerable<CommandMap> Maps { get; private set; }
        public TextWriter Output { get; private set; }

        public Application(
            string args,
            CommandBase command,
            bool enableMultiAction = true,
            TextWriter output = null,
            IMapper mapper = null,
            IExecutionStrategy executionStrategy = null,
            IExecutionListener executionListener = null
        ) : this(args, new List<CommandBase> { command }, enableMultiAction, output, mapper, executionStrategy, executionListener)
        {

        }

        public Application(
            string[] args,
            CommandBase command,
            bool enableMultiAction = true,
            TextWriter output = null,
            IMapper mapper = null,
            IExecutionStrategy executionStrategy = null,
            IExecutionListener executionListener = null
        ) : this(args, new List<CommandBase> { command }, enableMultiAction, output, mapper, executionStrategy, executionListener)
        {

        }

        public Application(
            string args,
            IEnumerable<CommandBase> commands,
            bool enableMultiAction = true,
            TextWriter output = null,
            IMapper mapper = null,
            IExecutionStrategy executionStrategy = null,
            IExecutionListener executionListener = null
        ) : this(AppHelpers.StringToArgs(args), commands, enableMultiAction, output, mapper, executionStrategy, executionListener)
        {

        }

        public Application(
            string[] args,
            IEnumerable<CommandBase> commands,
            bool enableMultiAction = true,
            TextWriter output = null,
            IMapper mapper = null,
            IExecutionStrategy executionStrategy = null,
            IExecutionListener executionListener = null
        )
        {
            commands = commands ?? new AppDomainCommandLoader().GetFromAppDomain(IsDebug);

            if (commands.Empty())
                throw new Exception("No command found");

            this.Output = output ?? Console.Out;
            this.Args = GetArguments(args);
            this.ArgsOriginal = this.Args;
            this.enableMultiAction = enableMultiAction;
            this.mapper = mapper ?? new DefaultMapper();
            this.executionStrategy = executionStrategy ?? new DefaultExecutionStrategy();
            this.executionListener = executionListener ?? new DefaultAppExecutionListener();
            this.Maps = this.mapper.CreateMap(commands);
        }

        public Result<IMember> Execute()
        {
            // system feature: "manage args history"
            var manageCommand = this.Maps.GetMap<IManageArgsHistoryCommand>();
            if (manageCommand != null)
            {
                var newArgs = new Evaluator(this.Args, manageCommand, false, this.executionStrategy, null).Execute().GetValue<string[]>();
                if (newArgs != null)
                    this.Args = newArgs;
            }

            // system feature: "help"
            var helpCommand = this.Maps.GetMap<IHelpCommand>();
            if (helpCommand != null)
            {
                var evaluator = new Evaluator(this.Args, helpCommand, false, this.executionStrategy, null);
                evaluator.Execute();
                if (evaluator.ExecutionState == ExecutionState.Success)
                    return evaluator.Result;
            }

            // execute user properties and methods
            var evaluatorUser = new Evaluator(this.Args, this.Maps.Where(f => f.Command.Tag == CommandTag.User), this.enableMultiAction, this.executionStrategy, this.executionListener);
            return evaluatorUser.Execute();
        }

        public static string[] GetArguments(string[] args = null)
        {
            if (Application.IsDebug && Application.ReadArgsWhenIsDebug)
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

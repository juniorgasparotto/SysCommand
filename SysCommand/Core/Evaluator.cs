using System;
using System.Collections.Generic;

namespace SysCommand
{
    public class Evaluator
    {
        private string[] args;
        private bool enableMultiAction;
        private IExecutionStrategy executionStrategy;
        private IExecutionListener executionListener;
        private IEnumerable<CommandMap> maps;
        private Result<IMember> result;

        public ExecutionState ExecutionState { get; private set; }

        public Result<IMember> Result
        {
            get
            {
                if (this.result == null)
                    this.result = this.executionStrategy.Parse(this.args, this.maps, this.enableMultiAction);
                return this.result;
            }
            private set
            {
                this.result = value;
            }
        }

        public Evaluator(
            string args,
            CommandMap map,
            bool enableMultiAction = true,
            IExecutionStrategy executionStrategy = null,
            IExecutionListener executionListener = null
        ) : this(args, new List<CommandMap> { map }, enableMultiAction, executionStrategy, executionListener)
        {

        }

        public Evaluator(
            string[] args,
            CommandMap map,
            bool enableMultiAction = true,
            IExecutionStrategy executionStrategy = null,
            IExecutionListener executionListener = null
        ) : this(args, new List<CommandMap> { map }, enableMultiAction, executionStrategy, executionListener)
        {

        }

        public Evaluator(
            string args,
            IEnumerable<CommandMap> commands,
            bool enableMultiAction = true,
            IExecutionStrategy executionStrategy = null,
            IExecutionListener executionListener = null
        ) : this(AppHelpers.StringToArgs(args), commands, enableMultiAction, executionStrategy, executionListener)
        {

        }

        public Evaluator(
            string[] args,
            IEnumerable<CommandMap> maps,
            bool enableMultiAction = true,
            IExecutionStrategy executionStrategy = null, 
            IExecutionListener executionListener = null
        )
        {
            if (maps.Empty())
                throw new Exception("No maps found");

            this.args = args ?? new string[0];
            this.maps = maps;
            this.enableMultiAction = enableMultiAction;
            this.executionStrategy = executionStrategy ?? new DefaultExecutionStrategy();
            this.executionListener = executionListener;
        }

        public Result<IMember> Execute()
        {
            try
            {
                this.SetSystemPropertiesInCommands();
                this.ExecutionState = this.executionStrategy.Execute(this.args, this.maps, this.Result);

                switch (this.ExecutionState)
                {
                    case ExecutionState.Success:
                        if (this.executionListener != null)
                            this.executionListener.OnSuccess(this.args, this.maps, this.Result);
                        break;
                    //case ExecutionState.ArgsIsEmpty:
                    //    if (this.executorListener != null)
                    //        this.executorListener.OnArgsIsEmpty(this.args, this.Map, this.Result);
                    //    break;
                    case ExecutionState.NotFound:
                        if (this.executionListener != null)
                            this.executionListener.OnNotFound(this.args, this.maps, this.Result);
                        break;
                    case ExecutionState.HasInvalidAction:
                        if (this.executionListener != null)
                            this.executionListener.OnInvalidArgumentParse(this.args, this.maps, this.Result);
                        break;
                    case ExecutionState.HasInvalidArgument:
                        if (this.executionListener != null)
                            this.executionListener.OnInvalidArgumentParse(this.args, this.maps, this.Result);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.ExecutionState = ExecutionState.UnexpectedError;

                if (this.executionListener != null)
                    this.executionListener.OnError(this.args, this.maps, this.Result, ex);
                else
                    throw ex;
            }

            // this return is only to mantain the fluent use
            return this.Result;
        }

        private void SetSystemPropertiesInCommands()
        {
            foreach (var command in this.maps)
            {
                command.Command.Args = this.args;
                command.Command.Maps = this.maps;
                command.Command.Result = this.Result;
            }
        }
    }
}

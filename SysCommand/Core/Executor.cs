using System;
using System.Collections.Generic;

namespace SysCommand
{
    public class Executor
    {
        public string[] args;
        public IEnumerable<Command> commands;
        public bool enableMultiAction;
        private IExecutionStrategy executionStrategy;
        private IExecutionListener executorListener;
        private Map map;
        private Result<IMember> result;

        public ExecutionState ExecutionState { get; private set; }

        public Map Map
        {
            get
            {
                if (this.map == null)
                    this.map = this.executionStrategy.CreateMap(this.commands);
                return this.map;
            }
            private set
            {
                this.map = value;
            }
        }

        public Result<IMember> Result
        {
            get
            {
                if (this.result == null)
                    this.result = this.executionStrategy.Parse(this.args, this.Map, this.enableMultiAction);
                return this.result;
            }
            private set
            {
                this.result = value;
            }
        }

        public Executor(
            string[] args,
            IEnumerable<Command> commands,
            IExecutionStrategy executionStrategy = null, 
            IExecutionListener executorListener = null
        )
        {
            if (commands.Empty())
                throw new Exception("No command found");

            this.args = args ?? new string[0];
            this.commands = commands;
            this.executionStrategy = executionStrategy ?? new DefaultExecutionStrategy();
            this.executorListener = executorListener;
        }

        public Result<IMember> Execute()
        {
            try
            {
                this.ExecutionState = this.executionStrategy.Execute(this.args, this.Map, this.Result);

                switch (this.ExecutionState)
                {
                    case ExecutionState.Success:
                        if (this.executorListener != null)
                            this.executorListener.OnSuccess(this.args, this.Map, this.Result);
                        break;
                    //case ExecutionState.ArgsIsEmpty:
                    //    if (this.executorListener != null)
                    //        this.executorListener.OnArgsIsEmpty(this.args, this.Map, this.Result);
                    //    break;
                    case ExecutionState.NotFound:
                        if (this.executorListener != null)
                            this.executorListener.OnNotFound(this.args, this.Map, this.Result);
                        break;
                    case ExecutionState.HasInvalidAction:
                        if (this.executorListener != null)
                            this.executorListener.OnInvalidArgumentParse(this.args, this.Map, this.Result);
                        break;
                    case ExecutionState.HasInvalidArgument:
                        if (this.executorListener != null)
                            this.executorListener.OnInvalidArgumentParse(this.args, this.Map, this.Result);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.ExecutionState = ExecutionState.UnexpectedError;

                if (this.executorListener != null)
                    this.executorListener.OnError(this.args, this.Map, this.Result, ex);
                else
                    throw ex;
            }

            // this return is only to mantain the fluent use
            return this.Result;
        }
    }
}

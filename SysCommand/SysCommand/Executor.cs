using System;
using System.Collections.Generic;
using System.Linq;

namespace SysCommand
{
    public class Executor
    {
        //private bool isExecuted = false;
        //private DefaultMapperStrategy mapper;
        //private DefaultParserStrategy parser;
        private IExecutionStrategy executionStrategy;
        private IExecutionListener executorListener;
        private Map map;
        private Result<IMember> result;

        public string[] Args { get; private set; }
        public IEnumerable<Command> Commands { get; private set; }
        public bool EnableMultiAction { get; private set; }
        public ExecutionState ReturnCode { get; set; }

        //public DefaultMapperStrategy MapperStrategy
        //{
        //    get
        //    {
        //        if (this.mapper == null)
        //            this.mapper = new DefaultMapperStrategy();
        //        return this.mapper;
        //    }
        //    private set
        //    {
        //        this.mapper = value;
        //    }
        //}

        //public DefaultParserStrategy ParserStrategy
        //{
        //    get
        //    {
        //        if (this.parser == null)
        //            this.parser = new DefaultParserStrategy();
        //        return this.parser;
        //    }
        //    private set
        //    {
        //        this.parser = value;
        //    }
        //}

        private IExecutionStrategy ExecutionStrategy
        {
            get
            {
                if (this.executionStrategy == null)
                    this.executionStrategy = new DefaultExecutionStrategy();
                return this.executionStrategy;
            }
            set
            {
                this.executionStrategy = value;
            }
        }

        //private IExecutorListener ExecutorListener
        //{
        //    get
        //    {
        //        return this.executorListener;
        //    }
        //    set
        //    {
        //        this.executorListener = value;
        //    }
        //}

        public Map Map
        {
            get
            {
                if (this.map == null)
                    this.map = this.ExecutionStrategy.CreateMap(this.Commands);
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
                    this.result = this.ExecutionStrategy.Parse(this.Args, this.Map, this.EnableMultiAction);
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
            //DefaultMapperStrategy mapperStrategy = null,
            //DefaultParserStrategy parserStrategy = null,
            IExecutionStrategy executionStrategy = null, 
            IExecutionListener executorListener = null
        )
        {
            if (commands.Empty())
                throw new Exception("No command found");

            this.Args = args ?? new string[0];
            this.Commands = commands;
            //this.MapperStrategy = mapperStrategy;
            //this.ParserStrategy = parserStrategy;
            this.ExecutionStrategy = executionStrategy;
            this.executorListener = executorListener;
        }

        public Result<IMember> Execute()
        {
            try
            {
                var executionState = this.ExecutionStrategy.Execute(this.Args, this.Map, this.Result);
                switch(executionState)
                {
                    case ExecutionState.Success:
                        if (this.executorListener != null)
                            this.executorListener.OnSuccess(this.Args, this.Map, this.Result);
                        break;
                    case ExecutionState.ArgsIsEmpty:
                        if (this.executorListener != null)
                            this.executorListener.OnArgsIsEmpty(this.Args, this.Map, this.Result);
                        break;
                    case ExecutionState.NotFound:
                        if (this.executorListener != null)
                            this.executorListener.OnNotFound(this.Args, this.Map, this.Result);
                        break;
                    case ExecutionState.HasInvalidAction:
                        if (this.executorListener != null)
                            this.executorListener.OnInvalidArgumentParse(this.Args, this.Map, this.Result);
                        break;
                    case ExecutionState.HasInvalidArgument:
                        if (this.executorListener != null)
                            this.executorListener.OnInvalidArgumentParse(this.Args, this.Map, this.Result);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (this.executorListener != null)
                    this.executorListener.OnError(this.Args, this.Map, this.Result, ex);
                else
                    throw ex;
            }

            return this.Result;
        }
    }
}

using System;
using System.Collections.Generic;

namespace SysCommand
{
    public class Evaluator
    {
        //public delegate void EvaluationExceptionArgs(string[] args, IEnumerable<CommandMap> maps, Result<IMember> result, Exception ex);
        //public delegate void EvaluationCompleteArgs(EvalState state, string[] args, IEnumerable<CommandMap> maps, Result<IMember> result);

        //private EvaluationCompleteArgs onCompleteCallBack;
        //private EvaluationExceptionArgs onExceptionCallBack;

        private string[] args;
        private bool enableMultiAction;
        private IEvaluationStrategy evaluationStrategy;
        private IEnumerable<CommandMap> maps;
        private Result<IMember> result;

        public EvalState EvalState { get; private set; }

        public Result<IMember> Result
        {
            get
            {
                if (this.result == null)
                    this.result = this.evaluationStrategy.Parse(this.args, this.maps, this.enableMultiAction);
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
            IEvaluationStrategy evaluationStrategy = null
        ) : this(args, new List<CommandMap> { map }, enableMultiAction, evaluationStrategy)
        {

        }

        public Evaluator(
            string[] args,
            CommandMap map,
            bool enableMultiAction = true,
            IEvaluationStrategy evaluationStrategy = null
        ) : this(args, new List<CommandMap> { map }, enableMultiAction, evaluationStrategy)
        {

        }

        public Evaluator(
            string args,
            IEnumerable<CommandMap> commands,
            bool enableMultiAction = true,
            IEvaluationStrategy evaluationStrategy = null
        ) : this(AppHelpers.StringToArgs(args), commands, enableMultiAction, evaluationStrategy)
        {

        }

        public Evaluator(
            string[] args,
            IEnumerable<CommandMap> maps,
            bool enableMultiAction = true,
            IEvaluationStrategy evaluationStrategy = null
        )
        {
            if (maps.Empty())
                throw new Exception("No maps found");

            this.args = args ?? new string[0];
            this.maps = maps;
            this.enableMultiAction = enableMultiAction;
            this.evaluationStrategy = evaluationStrategy ?? new DefaultEvaluationStrategy();
        }

        //public Evaluator OnComplete(EvaluationCompleteArgs onComplete)
        //{
        //    this.onCompleteCallBack = onComplete;
        //    return this;
        //}
       
        //public Evaluator OnException(EvaluationExceptionArgs onException)
        //{
        //    this.onExceptionCallBack = onException;
        //    return this;
        //}

        public Result<IMember> Eval()
        {
            //try
            //{
                this.SetSystemPropertiesInCommands();
                this.EvalState = this.evaluationStrategy.Eval(this.args, this.maps, this.Result);

                //if (this.onCompleteCallBack != null)
                //    this.onCompleteCallBack(this.EvalState, this.args, this.maps, this.Result);

                //switch (this.EvalState)
                //{
                //    case EvalState.Success:
                //        if (this.listener != null)
                //            this.listener.OnSuccess(this.args, this.maps, this.Result);
                //        break;
                //    //case ExecutionState.ArgsIsEmpty:
                //    //    if (this.executorListener != null)
                //    //        this.executorListener.OnArgsIsEmpty(this.args, this.Map, this.Result);
                //    //    break;
                //    case EvalState.NotFound:
                //        if (this.listener != null)
                //            this.listener.OnNotFound(this.args, this.maps, this.Result);
                //        break;
                //    case EvalState.HasInvalidAction:
                //        if (this.listener != null)
                //            this.listener.OnInvalidArgumentParse(this.args, this.maps, this.Result);
                //        break;
                //    case EvalState.HasInvalidArgument:
                //        if (this.listener != null)
                //            this.listener.OnInvalidArgumentParse(this.args, this.maps, this.Result);
                //        break;
                //}
            //}
            //catch (Exception ex)
            //{
            //    //this.EvalState = EvalState.UnexpectedError;

            //    //if (this.listener != null)
            //    //    this.listener.OnError(this.args, this.maps, this.Result, ex);
            //    //else
            //    //    throw ex;

            //    if (this.onExceptionCallBack != null)
            //        this.onExceptionCallBack(this.args, this.maps, this.Result, ex);
            //    else
            //        throw ex;
            //}

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

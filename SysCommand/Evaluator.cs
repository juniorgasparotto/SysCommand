using System;
using System.Collections.Generic;

namespace SysCommand
{
    public class Evaluator
    {
        private Action<IMember> onInvoke;

        private string[] args;
        private bool enableMultiAction;
        private IEvaluationStrategy evaluationStrategy;
        private IEnumerable<CommandMap> maps;
        private Result<IMember> result;

        public EvaluateState EvaluateState { get; private set; }

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

        public Evaluator OnInvoke(Action<IMember> onInvoke)
        {
            this.onInvoke = onInvoke;
            return this;
        }

        public Evaluator Evaluate()
        {
            this.SetSystemPropertiesInCommands();
            this.evaluationStrategy.OnInvoke = this.onInvoke;
            this.EvaluateState = this.evaluationStrategy.Evaluate(this.args, this.maps, this.Result);
            return this;
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

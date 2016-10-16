using System;
using System.Collections.Generic;

namespace SysCommand
{
    public class Evaluator
    {
        private Action<IMember> onInvoke;

        private string[] args;
        private bool enableMultiAction;
        private IEnumerable<CommandMap> maps;
        private ParseResult parseResult;
        private IEvaluationStrategy evaluationStrategy;

        public ParseResult ParseResult
        {
            get
            {
                if (this.parseResult == null)
                    this.parseResult = this.evaluationStrategy.Parse(this.args, this.maps, this.enableMultiAction);
                return this.parseResult;
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

        public EvaluateResult Evaluate()
        {
            this.evaluationStrategy.OnInvoke = this.onInvoke;
            var evaluateResult = this.evaluationStrategy.Evaluate(this.args, this.maps, this.parseResult);
            return evaluateResult;
        }
    }
}

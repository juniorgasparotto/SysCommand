using System.Collections.Generic;

namespace SysCommand.Evaluation
{
    public class EvaluateResult
    {
        public IEnumerable<IMemberResult> Results { get; set; }
        public IEnumerable<EvaluateError> Errors { get; set; }
        public EvaluateState State { get; set; }

        public EvaluateResult()
        {

        }
    }
}
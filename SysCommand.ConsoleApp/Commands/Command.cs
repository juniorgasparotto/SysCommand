using SysCommand.Parser;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SysCommand.ConsoleApp
{
    public abstract class Command : CommandBase
    {
        public App App { get; internal set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ActionMap CurrentMethodMap()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            var currentMethod = (MethodInfo)sf.GetMethod();
            return this.EvaluateScope.Maps.GetMap(this.GetType()).Methods.FirstOrDefault(m => AppHelpers.MethodsAreEquals(m.Method, currentMethod));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Method CurrentMethodParse()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            var currentMethod = (MethodInfo)sf.GetMethod();
            
            // return this method (eg: save) in all levels
            // -> save 1 2 3 save 4 5 6 save 7 8 9
            var allMethodResult = this.EvaluateScope.EvaluateResult.Result.With<Method>();
            var thisMethodForEachLevel = allMethodResult.Where(m => AppHelpers.MethodsAreEquals(m.ActionMapped.ActionMap.Method, currentMethod)).ToList();

            //  1.0) f.IsInvoked = false: save 1 2 3 -> FIRST: IsInvoked = FALSE
            //  1.1) f.IsInvoked = false: save 4 5 6 -> NO
            //  1.2) f.IsInvoked = false: save 7 8 9 -> NO
            //  2.0) f.IsInvoked = true:  save 1 2 3 -> NO
            //  2.1) f.IsInvoked = false: save 4 5 6 -> FIRST: IsInvoked = FALSE
            //  2.2) f.IsInvoked = false: save 7 8 9 -> NO
            //  3.0) f.IsInvoked = true:  save 1 2 3 -> NO
            //  3.1) f.IsInvoked = true:  save 4 5 6 -> NO
            //  3.2) f.IsInvoked = false: save 7 8 9 -> FIRST: IsInvoked = FALSE
            return thisMethodForEachLevel.First(f => !f.IsInvoked);
        }
    }
}

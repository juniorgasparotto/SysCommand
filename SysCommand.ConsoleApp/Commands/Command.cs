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
        public ActionMap GetCurrentMethodMap()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            var currentMethod = (MethodInfo)sf.GetMethod();
            return App.Maps.GetMap(this.GetType()).Methods.FirstOrDefault(m => AppHelpers.MethodsAreEquals(m.Method, currentMethod));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Method GetCurrentMethodResult()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            var currentMethod = (MethodInfo)sf.GetMethod();
            return this.Result.WithSource(this.GetType()).With<Method>().FirstOrDefault(m => AppHelpers.MethodsAreEquals(m.ActionMapped.ActionMap.Method, currentMethod));
        }

    }
}

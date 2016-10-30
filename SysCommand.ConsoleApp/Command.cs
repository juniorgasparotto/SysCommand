using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SysCommand.Mapping;
using SysCommand.Execution;
using SysCommand.Helpers;

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
            return this.ExecutionScope.ParseResult.Maps.GetMap(this.GetType()).Methods.FirstOrDefault(m => ReflectionHelper.MethodsAreEquals(m.Method, currentMethod));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public MethodResult CurrentMethodParse()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            var currentMethod = (MethodInfo)sf.GetMethod();
            
            // return this method (eg: save) in all levels
            // -> save 1 2 3 save 4 5 6 save 7 8 9
            var allMethodResult = this.ExecutionScope.ExecutionResult.Results.With<MethodResult>();
            var thisMethodForEachLevel = allMethodResult.Where(m => ReflectionHelper.MethodsAreEquals(m.ActionParsed.ActionMap.Method, currentMethod)).ToList();

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

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string View<T>(T model, bool searchInResourse = false)
        {
            var view = new View();
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            var executeInfo = new View.ExecuteInfo
            {
                Method = (MethodInfo)sf.GetMethod(),
                Type = this.GetType()
            };

            return view.ProcessByViewName<T>(model, executeInfo, searchInResourse);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string ViewContent<T>(T model, string content)
        {
            var view = new View();
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            return view.ProcessByContent<T>(model, content);
        }
    }
}

using SysCommand.Execution;
using SysCommand.Compatibility;
using System;
using System.Collections;
using System.Reflection;

namespace SysCommand.ConsoleApp.Handlers
{
    public class DefaultApplicationHandler : IApplicationHandler
    {
        /// <summary>
        /// Fires at the end of the implementation
        /// </summary>
        /// <param name="appResult">Result base</param>
        public virtual void OnComplete(ApplicationResult appResult)
        {
            switch (appResult.ExecutionResult.State)
            {
                case ExecutionState.Success:
                    if (!appResult.App.Console.ExitCodeHasValue)
                        appResult.App.Console.ExitCode = ExitCodeConstants.Success;
                    break;
                case ExecutionState.NotFound:
                    appResult.App.Console.ExitCode = ExitCodeConstants.Error;
                    appResult.App.Descriptor.ShowNotFound(appResult);
                    break;
                case ExecutionState.HasError:
                    appResult.App.Console.ExitCode = ExitCodeConstants.Error;
                    appResult.App.Descriptor.ShowErrors(appResult);
                    break;
            }
        }

        /// <summary>
        /// Fires in case of exception.
        /// </summary>
        /// <param name="appResult">Result base</param>
        /// <param name="ex">Last unhandled exception</param>
        public virtual void OnException(ApplicationResult appResult, Exception ex)
        {
            if (appResult.App.OnException.GetInvocationList().Length == 1)
                throw ex;
        }

        /// <summary>
        /// Fires before invoking each Member (property or method) that was parsed.
        /// </summary>
        /// <param name="appResult">Result base</param>
        /// <param name="member">Current member in execution</param>
        public virtual void OnBeforeMemberInvoke(ApplicationResult appResult, IMemberResult member)
        {
            
        }

        /// <summary>
        /// Fires after invoking each Member (property or method) that was parsed.
        /// </summary>
        /// <param name="appResult">Result base</param>
        /// <param name="member">Current member in execution</param>
        public virtual void OnAfterMemberInvoke(ApplicationResult appResult, IMemberResult member)
        {

        }

        /// <summary>
        /// Fires when a method returns value
        /// </summary>
        /// <param name="appResult">Result base</param>
        /// <param name="method">Current method in execution</param>
        public virtual void OnMethodReturn(ApplicationResult appResult, IMemberResult method)
        {
            if (method.Value != null)
            {
                if (method.Value.GetType() != typeof(string) && typeof(IEnumerable).IsAssignableFrom(method.Value.GetType()))
                {
                    foreach (var value in (IEnumerable)method.Value)
                        appResult.App.Descriptor.ShowMethodReturn(appResult, method, value);
                }
                else
                {
                    appResult.App.Descriptor.ShowMethodReturn(appResult, method, method.Value);
                }
            }
        }
    }
}
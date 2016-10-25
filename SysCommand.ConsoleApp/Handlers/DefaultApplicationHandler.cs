using SysCommand.Execution;
using System;
using System.Collections;

namespace SysCommand.ConsoleApp
{
    public class DefaultApplicationHandler : IApplicationHandler
    {
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

        public virtual void OnException(ApplicationResult appResult, Exception ex)
        {
            throw ex;
        }

        public virtual void OnBeforeMemberInvoke(ApplicationResult appResult, IMemberResult member)
        {
            
        }

        public virtual void OnAfterMemberInvoke(ApplicationResult appResult, IMemberResult member)
        {

        }

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
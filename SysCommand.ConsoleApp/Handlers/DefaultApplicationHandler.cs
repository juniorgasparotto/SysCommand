using SysCommand.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysCommand.ConsoleApp
{
    public class DefaultApplicationHandler : IApplicationHandler
    {
        public virtual void OnComplete(ApplicationResult appResult)
        {
            switch (appResult.EvaluateResult.State)
            {
                case EvaluateState.Success:
                    if (!appResult.App.Console.ExitCodeHasValue)
                        appResult.App.Console.ExitCode = ExitCodeConstants.Success;
                    break;
                case EvaluateState.NotFound:
                    appResult.App.Console.ExitCode = ExitCodeConstants.Error;
                    appResult.App.MessageFormatter.ShowNotFound(appResult);
                    break;
                case EvaluateState.HasError:
                    appResult.App.Console.ExitCode = ExitCodeConstants.Error;
                    appResult.App.MessageFormatter.ShowErrors(appResult);
                    break;
            }
        }

        public virtual void OnException(ApplicationResult appResult, Exception ex)
        {
            throw ex;
        }

        public virtual void OnBeforeMemberInvoke(ApplicationResult appResult, IMember member)
        {
            
        }

        public virtual void OnAfterMemberInvoke(ApplicationResult appResult, IMember member)
        {

        }

        public virtual void OnMethodReturn(ApplicationResult appResult, IMember method)
        {
            if (method.Value != null)
            {
                if (method.Value.GetType() != typeof(string) && typeof(IEnumerable).IsAssignableFrom(method.Value.GetType()))
                {
                    foreach (var value in (IEnumerable)method.Value)
                        appResult.App.Console.Write(value);
                }
                else
                {
                    appResult.App.Console.Write(method.Value);
                }
            }
        }
    }
}
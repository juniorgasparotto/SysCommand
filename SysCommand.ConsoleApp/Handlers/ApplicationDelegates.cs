using SysCommand.Execution;
using System;

namespace SysCommand.ConsoleApp.Handlers
{
    public delegate void AppRunExceptionHandler(ApplicationResult applicationResult, Exception ex);
    public delegate void AppRunCompleteHandler(ApplicationResult applicationResult);
    public delegate void AppRunOnBeforeMemberInvokeHandler(ApplicationResult applicationResult, IMemberResult member);
    public delegate void AppRunOnAfterMemberInvokeHandler(ApplicationResult applicationResult, IMemberResult member);
    public delegate void AppRunOnMethodReturnHandler(ApplicationResult applicationResult, IMemberResult member);
}

using System;

namespace SysCommand.ConsoleApp
{
    public delegate void AppRunExceptionHandler(ApplicationResult args, Exception ex);
    public delegate void AppRunCompleteHandler(ApplicationResult args);
    public delegate void AppRunOnBeforeMemberInvokeHandler(ApplicationResult args, IMember member);
    public delegate void AppRunOnAfterMemberInvokeHandler(ApplicationResult args, IMember member);
    public delegate void AppRunOnMethodReturnHandler(ApplicationResult args, IMember member);
}

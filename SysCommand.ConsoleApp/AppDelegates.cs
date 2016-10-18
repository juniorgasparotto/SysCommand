using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Reflection;

namespace SysCommand.ConsoleApp
{
    public delegate void AppRunExceptionHandler(AppResult args, Exception ex);
    public delegate void AppRunCompleteHandler(AppResult args);
    public delegate void AppRunOnBeforeMemberInvokeHandler(AppResult args, IMember member);
    public delegate void AppRunOnAfterMemberInvokeHandler(AppResult args, IMember member);
    public delegate void AppRunOnMethodReturnHandler(AppResult args, IMember member);
}

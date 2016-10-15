using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.Tests.ConsoleApp.Commands;
using SysCommand.ConsoleApp;
using System.IO;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestAppEvents
    {
        public class CustomListener : IEventListener
        {
            public void OnComplete(AppEventsArgs eventArgs)
            {
                eventArgs.App.Console.Write("OnComplete");
                throw new Exception("Exception!!");
            }

            public void OnException(AppEventsArgs eventArgs, Exception ex)
            {
                eventArgs.App.Console.Write(string.Format("OnException: {0}", ex.Message));
            }

            public void OnBeforeMemberInvoke(AppEventsArgs eventArgs, IMember member)
            {
                eventArgs.App.Console.Write(string.Format("OnBeforeMemberInvoke: {0} {1}", member.Name, member.Value));
            }

            public void OnAfterMemberInvoke(AppEventsArgs eventArgs, IMember member)
            {
                eventArgs.App.Console.Write(string.Format("OnAfterMemberInvoke: {0}: {1}", member.Name, member.Value));
            }

            public void OnPrint(AppEventsArgs eventArgs, IMember method)
            {
                eventArgs.App.Console.Write(string.Format("OnPrint: {0}: {1}", method.Name, method.Value));
            }
        }

        [TestMethod]
        public void TestEventsWithClass()
        {
            var listener = new CustomListener();
            var app = new App(
                    commands: new List<SysCommand.ConsoleApp.Command> { new OnlyMainCommand() },
                    listener: listener
                );

            app.Console.Out = new StringWriter();
            app.Run("-a Y");

            var output = app.Console.Out.ToString();
            var expected =
@"OnBeforeMemberInvoke: a Y
OnAfterMemberInvoke: a: Y
OnBeforeMemberInvoke: Main 
OnAfterMemberInvoke: Main: Main
OnPrint: Main: Main
OnComplete
OnException: Exception!!";

            Assert.IsTrue(expected == output);
        }

        [TestMethod]
        public void TestEventsWithActions()
        {
            var app = new App(
                    commands: new List<SysCommand.ConsoleApp.Command> { new OnlyMainCommand() }
                )
            .OnComplete((args) => { args.App.Console.Write("ActionsOnComplete"); throw new Exception("Exception!!"); })
            .OnException((args, ex) => args.App.Console.Write(string.Format("ActionsOnException: {0}", ex.Message)))
            .OnBeforeMemberInvoke((args, member) => args.App.Console.Write(string.Format("ActionsOnBeforeMemberInvoke: {0}: {1}", member.Name, member.Value)))
            .OnAfterMemberInvoke((args, member) => args.App.Console.Write(string.Format("ActionsOnAfterMemberInvoke: {0}: {1}", member.Name, member.Value)))
            .OnPrint((args, member) => args.App.Console.Write(string.Format("ActionsOnPrint: {0}: {1}", member.Name, member.Value)));

            app.Console.Out = new StringWriter();
            app.Run("-a Y");

            var output = app.Console.Out.ToString();
            var expected =
@"ActionsOnBeforeMemberInvoke: a: Y
ActionsOnAfterMemberInvoke: a: Y
ActionsOnBeforeMemberInvoke: Main: 
ActionsOnAfterMemberInvoke: Main: Main
ActionsOnPrint: Main: Main
ActionsOnComplete
ActionsOnException: Exception!!";
            Assert.IsTrue(expected == output);
        }
    }
}

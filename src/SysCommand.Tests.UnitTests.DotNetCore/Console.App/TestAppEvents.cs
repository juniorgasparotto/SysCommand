﻿using System.Collections.Generic;
using System;
using SysCommand.ConsoleApp;
using System.IO;
using SysCommand.Execution;
using SysCommand.ConsoleApp.Handlers;
using Xunit;
using SysCommand.Tests.UnitTests.Common;

namespace SysCommand.Tests.UnitTests
{
    public class TestAppCallBacks
    {
        public TestAppCallBacks()
        {
            TestHelper.Setup();
        }

        public class CustomListener : IApplicationHandler
        {
            public void OnComplete(ApplicationResult eventArgs)
            {
                eventArgs.App.Console.Write("OnComplete");
                throw new Exception("Exception!!");
            }

            public void OnException(ApplicationResult eventArgs, Exception ex)
            {
                eventArgs.App.Console.Write(string.Format("OnException: {0}", ex.Message));
            }

            public void OnBeforeMemberInvoke(ApplicationResult eventArgs, IMemberResult member)
            {
                eventArgs.App.Console.Write(string.Format("OnBeforeMemberInvoke: {0} {1}", member.Name, member.Value));
            }

            public void OnAfterMemberInvoke(ApplicationResult eventArgs, IMemberResult member)
            {
                eventArgs.App.Console.Write(string.Format("OnAfterMemberInvoke: {0}: {1}", member.Name, member.Value));
            }

            public void OnMethodReturn(ApplicationResult eventArgs, IMemberResult method)
            {
                eventArgs.App.Console.Write(string.Format("OnPrint: {0}: {1}", method.Name, method.Value));
            }
        }

        [Fact]
        public void Test22_EventsWithClass()
        {
            var listener = new CustomListener();
            var app = new App(
                    commandsTypes: new List<Type> { typeof(Common.Commands.T22.Command1) },
                    addDefaultAppHandler: false
                );

            app.AddApplicationHandler(listener);
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

            Assert.True(TestHelper.CompareString(expected, output));
        }

        [Fact]
        public void Test22_EventsWithActions()
        {
            var app = new App(
                    commandsTypes: new List<Type> { typeof(Common.Commands.T22.Command1) },
                    addDefaultAppHandler: false
                );

            app.OnComplete += (args) => { args.App.Console.Write("ActionsOnComplete"); throw new Exception("Exception!!"); };
            app.OnException += (args, ex) => args.App.Console.Write(string.Format("ActionsOnException: {0}", ex.Message));
            app.OnBeforeMemberInvoke += (args, member) => args.App.Console.Write(string.Format("ActionsOnBeforeMemberInvoke: {0}: {1}", member.Name, member.Value));
            app.OnAfterMemberInvoke += (args, member) => args.App.Console.Write(string.Format("ActionsOnAfterMemberInvoke: {0}: {1}", member.Name, member.Value));
            app.OnMethodReturn += (args, member) => args.App.Console.Write(string.Format("ActionsOnPrint: {0}: {1}", member.Name, member.Value));

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
            Assert.True(TestHelper.CompareString(expected, output));
        }
    }
}

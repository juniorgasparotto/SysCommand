using System.Text;
using SysCommand.ConsoleApp.View;
using SysCommand.DefaultExecutor;
using SysCommand.Mapping;
using Xunit;
using SysCommand.Tests.UnitTests.Common;
using SysCommand.ConsoleApp;
using System.Linq;
using System.IO;
using System;
using SysCommand.Tests.UnitTests.Common.Commands.ViewModels;
using System.Collections.Generic;

namespace SysCommand.Tests.UnitTests
{
    public class RazorViewTest
    {
        IEnumerable<Type> cmds = GetCmds(
                        new Command[]
                        {
                            new RazorCommand()
                        }
                    ).Select(f => f.GetType());

        public RazorViewTest()
        {
            TestHelper.Setup();
        }

        #region Razor by string

        [Fact]
        public void RazorView_WithContentWithoutModel()
        {
            var strBuilder = new StringWriter();
            var app = new App(cmds, output: strBuilder);
            var args = "with-content-without-model";
            var appResult = app.Run(args);
            var output = app.Console.Out.ToString().Trim();
            Assert.Equal("WithoutModel", output);
        }

        [Fact]
        public void RazorView_WithContentWithModel()
        {
            var strBuilder = new StringWriter();
            var app = new App(cmds, output: strBuilder);
            var args = "with-content-with-model";
            var appResult = app.Run(args);
            var output = app.Console.Out.ToString().Trim();
            Assert.Equal("With: MyName", output);
        }

        [Fact]
        public void RazorView_WithContentWithoutModelAsync()
        {
            var strBuilder = new StringWriter();
            var app = new App(cmds, output: strBuilder);
            var args = "with-content-without-model-async";
            var appResult = app.Run(args);
            var output = app.Console.Out.ToString().Trim();
            Assert.Equal("WithoutModel", output);
        }

        [Fact]
        public void RazorView_WithContentWithModelAsync()
        {
            var strBuilder = new StringWriter();
            var app = new App(cmds, output: strBuilder);
            var args = "with-content-with-model-async";
            var appResult = app.Run(args);
            var output = app.Console.Out.ToString().Trim();
            Assert.Equal("With: MyName", output);
        }

#endregion

#region Razor with file system and no view specify

        [Fact]
        public void RazorView_WithFileSystemWithoutViewNameWithoutModel()
        {
            var strBuilder = new StringWriter();
            var app = new App(cmds, output: strBuilder);
            var args = "with-file-system-without-view-name-without-model";
            var appResult = app.Run(args);
            var output = app.Console.Out.ToString().Trim();
            Assert.Equal(@"Inherits method: CustomText
Inherits property: CustomText
Datetime: True
Explicit: -2
WriteHtmlInString: <span>Hello World</span>
From method: Hello", output);
        }

        [Fact]
        public void RazorView_WithFileSystemWithoutViewNameWithDynamicModel()
        {
            var strBuilder = new StringWriter();
            var app = new App(cmds, output: strBuilder);
            var args = "with-file-system-without-view-name-with-dynamic-model";
            var appResult = app.Run(args);
            var output = app.Console.Out.ToString().Trim();
            Assert.Equal(@"Scaped Value: MyName", output);
        }

        [Fact]
        public void RazorView_WithFileSystemWithoutViewNameWithoutModelAsync()
        {
            var strBuilder = new StringWriter();
            var app = new App(cmds, output: strBuilder);
            var args = "with-file-system-without-view-name-without-model-async";
            var appResult = app.Run(args);
            var output = app.Console.Out.ToString().Trim();
            Assert.Equal(@"Inherits method: CustomText
Inherits property: CustomText
Datetime: True
Explicit: -2
WriteHtmlInString: <span>Hello World</span>
From method: Hello", output);
        }

        [Fact]
        public void RazorView_WithFileSystemWithoutViewNameWithDynamicModelAsync()
        {
            var strBuilder = new StringWriter();
            var app = new App(cmds, output: strBuilder);
            var args = "with-file-system-without-view-name-with-dynamic-model-async";
            var appResult = app.Run(args);
            var output = app.Console.Out.ToString().Trim();
            Assert.Equal(@"Scaped Value: MyName", output);
        }

#endregion

#region Razor with file system and view name specify

        [Fact]
        public void RazorView_WithFileSystemViewNameWithModel()
        {
            var strBuilder = new StringWriter();
            var app = new App(cmds, output: strBuilder);
            var args = "with-file-system-view-name-with-model";
            var appResult = app.Run(args);
            var output = app.Console.Out.ToString().Trim();
            Assert.Equal(@"Model: MyName", output);
        }

#endregion

#region Razor with embedded view

        [Fact]
        public void RazorView_WithEmbeddedWithoutViewNameWithoutModel()
        {
            var strBuilder = new StringWriter();
            var app = new App(cmds, output: strBuilder);
            var args = "with-embedded-without-view-name-without-model";
            var appResult = app.Run(args);
            var output = app.Console.Out.ToString().Trim();
            Assert.Equal(@"Model: MyName", output);
        }

        [Fact]
        public void RazorView_WithEmbeddedWithViewNameWithoutModel()
        {
            var strBuilder = new StringWriter();
            var app = new App(cmds, output: strBuilder);
            var args = "with-embedded-with-view-name-without-model";
            var appResult = app.Run(args);
            var output = app.Console.Out.ToString().Trim();
            Assert.Equal(@"Model: MyName", output);
        }

#endregion

        private static Command[] GetCmds(params Command[] command)
        {
            return CompareHelper.GetCmds(command);
        }
    }
}

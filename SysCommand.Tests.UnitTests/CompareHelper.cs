using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.ConsoleApp;
using System.IO;
using SysCommand.Test;

namespace SysCommand.Tests.UnitTests
{
    public static class CompareHelper
    {
        public static TestData GetTestData(string args, Command[] commands, StringWriter strBuilder = null)
        {
            var app = new App(
                    commandsTypes: commands.Select(f=>f.GetType())
                );

            app.Console.Out = strBuilder ?? new StringWriter();

            var appResult = app.Run(args);
            var output = app.Console.Out.ToString();

            var test = new TestData();
            test.Args = args;

            foreach (var cmd in commands)
            {
                test.Members.AddRange(app.Maps.Where(f => f.Command.GetType() == cmd.GetType()).SelectMany(f => f.Properties.Select(s => s.Target.GetType().Name + "." + s.TargetMember.ToString() + (s.IsOptional ? "" : " (obrigatory)") + (cmd.EnablePositionalArgs ? "" : " (NOT accept positional)"))));
                test.Members.AddRange(app.Maps.Where(f => f.Command.GetType() == cmd.GetType()).SelectMany(f => f.Methods.Select(s => s.Target.GetType().Name + "." + CommandParserUtils.GetMethodSpecification(s))));
            }

            test.ExpectedResult = output.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            test.Values = appResult.ExecutionResult.Results.Select(f => f.Target.GetType().Name + "." + f.Name + "=" + f.Value);

            foreach (var level in appResult.ParseResult.Levels)
            {
                var testLevel = new TestData.Level();
                testLevel.LevelNumber = level.LevelNumber;
                test.Levels.Add(testLevel);

                testLevel.CommandsValid.AddRange(level.Commands.Where(f => f.IsValid).Select(f => f.Command.GetType().Name));
                testLevel.CommandsWithError.AddRange(level.Commands.Where(f => f.HasError).Select(f => f.Command.GetType().Name));

                foreach (var cmd in level.Commands)
                {
                    testLevel.MethodsValid.AddRange(cmd.Methods.Select(s => cmd.Command.GetType().Name + "." + CommandParserUtils.GetMethodSpecification(s.ActionMap)));
                    testLevel.MethodsInvalid.AddRange(cmd.MethodsInvalid.Select(s => cmd.Command.GetType().Name + "." + CommandParserUtils.GetMethodSpecification(s.ActionMap)));
                    testLevel.PropertiesValid.AddRange(cmd.Properties.Select(s => cmd.Command.GetType().Name + "." + s.Map.TargetMember.ToString()));
                    testLevel.PropertiesInvalid.AddRange(cmd.PropertiesInvalid.Select(s => cmd.Command.GetType().Name + "." + (s.Name ?? s.Value)));
                }
            }

            return test;
        }

        public static TestData Compare<T>(string args, Command[] commands, string funcName)
        {
            var test = GetTestData(args, commands);
            Assert.IsTrue(TestHelper.CompareObjects<T>(test, null, funcName));
            return test;
        }

        public class TestData
        {
            public class Level
            {
                public int LevelNumber { get; internal set; }
                public List<string> CommandsValid { get; set; }
                public List<string> PropertiesValid { get; internal set; }
                public List<string> MethodsValid { get; internal set; }
                public List<string> CommandsWithError { get; set; }
                public List<string> PropertiesInvalid { get; internal set; }
                public List<string> MethodsInvalid { get; internal set; }

                public Level()
                {
                    this.MethodsValid = new List<string>();
                    this.MethodsInvalid = new List<string>();
                    this.PropertiesValid = new List<string>();
                    this.PropertiesInvalid = new List<string>();
                    this.CommandsValid = new List<string>();
                    this.CommandsWithError = new List<string>();
                }
            }

            public string Args { get; internal set; }
            public List<string> Members { get; internal set; }
            public IEnumerable<object> Values { get; internal set; }
            public string[] ExpectedResult { get; set; }
            public List<Level> Levels { get; internal set; }

            public TestData()
            {
                this.Members = new List<string>();
                this.Levels = new List<Level>();
            }
        }

        public static Command[] GetCmds(params Command[] commands)
        {
            return commands;
        }
    }
}

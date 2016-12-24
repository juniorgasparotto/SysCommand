using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.ConsoleApp;
using System.IO;
using SysCommand.Test;
using SysCommand.ConsoleApp.Commands;
using System.Text;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestAppMultiCommand
    {
        public TestAppMultiCommand()
        {
            TestHelper.SetCultureInfoToInvariant();
        }

        [TestMethod]
        public void Test28_2CommandsAndWithDiffSignature()
        {
            this.CompareAll(
                    args: "1 2 3 4",
                    commands: GetCmds(
                        new Commands.T28.Command1(),
                        new Commands.T28.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test32_2Commands1InvalidDefaultMethodAnd1ValidProperties_ReadProperty()
        {
            this.CompareAll(
                    args: "--prop1 value",
                    commands: GetCmds(
                        new Commands.T32.Command1(),
                        new Commands.T32.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test32_2Commands1InvalidDefaultMethodAnd1ValidPropertiesExplicit_Error()
        {
            this.CompareAll(
                    args: "default --prop1 value",
                    commands: GetCmds(
                        new Commands.T32.Command1(),
                        new Commands.T32.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test32_2Commands1ValidDefaultMethod_ReadMethod()
        {
            this.CompareAll(
                    args: "--value value",
                    commands: GetCmds(
                        new Commands.T32.Command1(),
                        new Commands.T32.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test32_2Commands1ValidDefaultMethodAnd1ValidProperties_ReadBoth()
        {
            this.CompareAll(
                    args: "--value value --prop1 value",
                    commands: GetCmds(
                        new Commands.T32.Command1(),
                        new Commands.T32.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test32_2Commands1ValidDefaultMethodAnd1ValidProperties_ReadBoth2()
        {
            this.CompareAll(
                    args: "--prop1 value --value value",
                    commands: GetCmds(
                        new Commands.T32.Command1(),
                        new Commands.T32.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test32_2Commands1ValidDefaultMethodAnd1ValidPropertiesPositional_ReadBoth()
        {
            this.CompareAll(
                    args: "1 2",
                    commands: GetCmds(
                        new Commands.T32.Command2(),
                        new Commands.T32.Command3()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test33_5CommandsWithDefaultMethods_Exec2MethodsAnd1Properties()
        {
            this.CompareAll(
                    args: "--p1 1 --description bla -c bla",
                    commands: GetCmds(
                        new Commands.T33.Command1(),
                        new Commands.T33.Command2(),
                        new Commands.T33.Command3(),
                        new Commands.T33.Command4(),
                        new Commands.T33.Command5()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test34_3CommandsWith3Properties_Exec3Properties()
        {
            this.CompareAll(
                    args: "--a1 1 --b1 2",
                    commands: GetCmds(
                        new Commands.T34.Command3(),
                        new Commands.T34.Command1(),
                        new Commands.T34.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test35_2CommandsWithListAnd4Args_ExecMethodWith4Args()
        {
            this.CompareAll(
                    args: "1 2 3 4",
                    commands: GetCmds(
                        new Commands.T35.Command1(),
                        new Commands.T35.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test35_2CommandsWithListAnd4Args_ExecMethodWithList()
        {
            this.CompareAll(
                    args: "1 2 3 4 5",
                    commands: GetCmds(
                        new Commands.T35.Command1(),
                        new Commands.T35.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test36_2CommandsWithListAndArgs_ExecMethodWithList()
        {
            this.CompareAll(
                    args: "1 2 3 4 5",
                    commands: GetCmds(
                        new Commands.T36.Command1(),
                        new Commands.T36.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test36_2CommandsWithListAndArgs_ExecMethodWithList2()
        {
            this.CompareAll(
                    args: "--b1 1 2 3 4 5",
                    commands: GetCmds(
                        new Commands.T36.Command1(),
                        new Commands.T36.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [TestMethod]
        public void Test37_2CommandsWithListAndArgs_ExecMethodWithList3()
        {
            this.CompareAll(
                    args: "--a1 1 2 3 4 5",
                    commands: GetCmds(
                        new Commands.T37.Command1(),
                        new Commands.T37.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }


        private void CompareAll(string args, Command[] commands, string funcName)
        {
            CompareHelper.Compare<TestAppMultiCommand>(args, commands, funcName);
        }

        private Command[] GetCmds(params Command[] command)
        {
            return CompareHelper.GetCmds(command);
        }
    }
}

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SysCommand.Tests;

namespace SysCommand.UnitTests
{
    [TestClass]
    public class TestRun
    {
        #region Aux
        public TestRun()
        {
            // Only to load all actions in 'console application test project'.
            Program.LoadAssembly();
        }

        private void InitializeApp(string args, StringWriter output = null, char? prefix = null)
        {
            if (output != null)
                Console.SetOut(output);

            App.Initialize();
            App.Current.DebugShowExitConfirm = false;
            App.Current.ActionCharPrefix = prefix;
            App.Current.SetArgs(args);
            App.Current.Run();
        }
        #endregion

        [TestMethod]
        public void TestHelp()
        {
            var output = new StringWriter();
            this.InitializeApp("--help", output);
            var strOutput  = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestNoActionToResponse()
        {
            var output = new StringWriter();
            this.InitializeApp("save", output);
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        #region Without prefix

        [TestMethod]
        public void TestActionNotExists()
        {
            var output = new StringWriter();
            this.InitializeApp("save-not-exists --p1 \"a\" --p2 \"b\"", output);
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestArgPosition()
        {
            var output = new StringWriter();
            this.InitializeApp(@"position --key+ \--value1 \--value2", output);
            //this.InitializeApp(@"position --key- \--value1 \--value2", output);
            //this.InitializeApp(@"position --key \--value1 \--value2", output);
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");

            output = new StringWriter();
            this.InitializeApp(@"position --key \--value1 \--value2", output);
            strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestActionExists()
        {
            var output = new StringWriter();
            this.InitializeApp("save --p1 \"a\" --p2 \"b\"", output);
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestActionExistsButScaped()
        {
            var output = new StringWriter();
            this.InitializeApp(@"\save --p1 ""a"" --p2 ""b""", output);
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestActionNotExistsAndScaped()
        {
            var output = new StringWriter();
            this.InitializeApp(@"\save-not-exists --p1 ""a"" --p2 ""b""", output);
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestActionNotExistsButNotScaped()
        {
            var output = new StringWriter();
            this.InitializeApp(@"save-not-exists --p1 ""a"" --p2 ""b""", output);
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        #endregion

        #region With prefix

        [TestMethod]
        public void TestPrefixedActionExistsButWithoutPrefix()
        {
            var output = new StringWriter();
            this.InitializeApp(@"save --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a", output, '$');
            var strOutput  = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestPrefixedActionExistsAndScapedButWithoutPrefix()
        {
            var output = new StringWriter();
            this.InitializeApp(@"\save --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a", output, '$');
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestPrefixedActionNotExistsAndScapedButWithoutPrefix()
        {
            var output = new StringWriter();
            this.InitializeApp(@"\save-not-exists --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a", output, '$');
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestPrefixedActionNotExistsButWithoutPrefix()
        {
            var output = new StringWriter();
            this.InitializeApp(@"save-not-exists --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a", output, '$');
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestPrefixedActionExists()
        {
            var output = new StringWriter();
            this.InitializeApp(@"$save --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a \$save-not-exists --p1 a $save-not-exists $delete --p1", output, '$');
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestPrefixedActionExistsButScaped()
        {
            var output = new StringWriter();
            this.InitializeApp(@"\$save --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a \$save-not-exists --p1 a $save-not-exists $delete --p1", output, '$');
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestPrefixedActionNotExists()
        {
            var output = new StringWriter();
            this.InitializeApp(@"$save-not-exists --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a \$save-not-exists --p1 a $save-not-exists $delete --p1", output, '$');
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }

        [TestMethod]
        public void TestPrefixedActionNotExistsAndScaped()
        {
            var output = new StringWriter();
            this.InitializeApp(@"\$save-not-exists --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a \$save-not-exists --p1 a $save-not-exists $delete --p1", output, '$');
            var strOutput = output.ToString().Trim();
            Assert.IsTrue(strOutput == "Executing: TaskCommand/Save");
        }
        #endregion
    }
}

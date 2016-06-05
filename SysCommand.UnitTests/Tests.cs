using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SysCommand.Tests;

namespace SysCommand.UnitTests
{
    [TestClass]
    public class Tests
    {
        #region Aux
        public Tests()
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
        public void TestRun()
        {
            var output = new StringWriter();
            this.InitializeApp("save");
            output.ToString();
        }

        #region Without prefix

        [TestMethod]
        public void TestRequestActionNotExists()
        {
            this.InitializeApp("save-not-exists --p1 \"a\" --p2 \"b\"");
            var request = App.Current.Request;
            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == null);
            Assert.IsTrue(request.RequestActions.Count == 1);
            Assert.IsTrue(requestAction.Name == null);
            Assert.IsTrue(requestAction.Arguments[0] == "save-not-exists");
            Assert.IsTrue(requestAction.Arguments[1] == "--p1");
            Assert.IsTrue(requestAction.Arguments[2] == "a");
            Assert.IsTrue(requestAction.Arguments[3] == "--p2");
            Assert.IsTrue(requestAction.Arguments[4] == "b");
        }

        [TestMethod]
        public void TestRequestActionExists()
        {
            this.InitializeApp("save --p1 \"a\" --p2 \"b\"");
            var request = App.Current.Request;
            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == "save");
            Assert.IsTrue(request.RequestActions.Count == 1);
            Assert.IsTrue(requestAction.Name == "save");
            Assert.IsTrue(requestAction.Arguments[0] == "--p1");
            Assert.IsTrue(requestAction.Arguments[1] == "a");
            Assert.IsTrue(requestAction.Arguments[2] == "--p2");
            Assert.IsTrue(requestAction.Arguments[3] == "b");
        }

        [TestMethod]
        public void TestRequestActionExistsButScaped()
        {
            this.InitializeApp(@"\save --p1 ""a"" --p2 ""b""");
            var request = App.Current.Request;
            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == null);
            Assert.IsTrue(request.RequestActions.Count == 1);
            Assert.IsTrue(requestAction.Name == null);
            Assert.IsTrue(requestAction.Arguments[0] == "save");
            Assert.IsTrue(requestAction.Arguments[1] == "--p1");
            Assert.IsTrue(requestAction.Arguments[2] == "a");
            Assert.IsTrue(requestAction.Arguments[3] == "--p2");
            Assert.IsTrue(requestAction.Arguments[4] == "b");
        }

        [TestMethod]
        public void TestRequestActionNotExistsAndScaped()
        {
            this.InitializeApp(@"\save-not-exists --p1 ""a"" --p2 ""b""");
            var request = App.Current.Request;
            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == null);
            Assert.IsTrue(request.RequestActions.Count == 1);
            Assert.IsTrue(requestAction.Name == null);
            Assert.IsTrue(requestAction.Arguments[0] == @"\save-not-exists");
            Assert.IsTrue(requestAction.Arguments[1] == "--p1");
            Assert.IsTrue(requestAction.Arguments[2] == "a");
            Assert.IsTrue(requestAction.Arguments[3] == "--p2");
            Assert.IsTrue(requestAction.Arguments[4] == "b");
        }

        [TestMethod]
        public void TestRequestActionNotExistsButNotScaped()
        {
            this.InitializeApp(@"save-not-exists --p1 ""a"" --p2 ""b""");
            var request = App.Current.Request;
            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == null);
            Assert.IsTrue(request.RequestActions.Count == 1);
            Assert.IsTrue(requestAction.Name == null);
            Assert.IsTrue(requestAction.Arguments[0] == @"save-not-exists");
            Assert.IsTrue(requestAction.Arguments[1] == "--p1");
            Assert.IsTrue(requestAction.Arguments[2] == "a");
            Assert.IsTrue(requestAction.Arguments[3] == "--p2");
            Assert.IsTrue(requestAction.Arguments[4] == "b");
        }

        #endregion

        #region With prefix

        [TestMethod]
        public void TestPrefixedRequestActionExistsButWithoutPrefix()
        {
            this.InitializeApp(@"save --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a", null, '$');
            var request = App.Current.Request;
            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == null);
            Assert.IsTrue(request.RequestActions.Count == 1);
            Assert.IsTrue(requestAction.Name == null);
            Assert.IsTrue(requestAction.Arguments[0] == "save");
            Assert.IsTrue(requestAction.Arguments[1] == "--p1");
            Assert.IsTrue(requestAction.Arguments[2] == "a");
            Assert.IsTrue(requestAction.Arguments[3] == "--p2");
            Assert.IsTrue(requestAction.Arguments[4] == "b");
            Assert.IsTrue(requestAction.Arguments[5] == "$save");
            Assert.IsTrue(requestAction.Arguments[6] == "--p1");
            Assert.IsTrue(requestAction.Arguments[7] == "a");
            Assert.IsTrue(requestAction.Arguments[8] == @"\$save");
            Assert.IsTrue(requestAction.Arguments[9] == "--p1");
            Assert.IsTrue(requestAction.Arguments[10] == "a");
        }

        [TestMethod]
        public void TestPrefixedRequestActionExistsAndScapedButWithoutPrefix()
        {
            this.InitializeApp(@"\save --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a", null, '$');
            var request = App.Current.Request;
            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == null);
            Assert.IsTrue(request.RequestActions.Count == 1);
            Assert.IsTrue(requestAction.Name == null);
            Assert.IsTrue(requestAction.Arguments[0] == @"\save");
            Assert.IsTrue(requestAction.Arguments[1] == "--p1");
            Assert.IsTrue(requestAction.Arguments[2] == "a");
            Assert.IsTrue(requestAction.Arguments[3] == "--p2");
            Assert.IsTrue(requestAction.Arguments[4] == "b");
            Assert.IsTrue(requestAction.Arguments[5] == "$save");
            Assert.IsTrue(requestAction.Arguments[6] == "--p1");
            Assert.IsTrue(requestAction.Arguments[7] == "a");
            Assert.IsTrue(requestAction.Arguments[8] == @"\$save");
            Assert.IsTrue(requestAction.Arguments[9] == "--p1");
            Assert.IsTrue(requestAction.Arguments[10] == "a");
        }

        [TestMethod]
        public void TestPrefixedRequestActionNotExistsAndScapedButWithoutPrefix()
        {
            this.InitializeApp(@"\save-not-exists --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a", null, '$');
            var request = App.Current.Request;
            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == null);
            Assert.IsTrue(request.RequestActions.Count == 1);
            Assert.IsTrue(requestAction.Name == null);
            Assert.IsTrue(requestAction.Arguments[0] == @"\save-not-exists");
            Assert.IsTrue(requestAction.Arguments[1] == "--p1");
            Assert.IsTrue(requestAction.Arguments[2] == "a");
            Assert.IsTrue(requestAction.Arguments[3] == "--p2");
            Assert.IsTrue(requestAction.Arguments[4] == "b");
            Assert.IsTrue(requestAction.Arguments[5] == "$save");
            Assert.IsTrue(requestAction.Arguments[6] == "--p1");
            Assert.IsTrue(requestAction.Arguments[7] == "a");
            Assert.IsTrue(requestAction.Arguments[8] == @"\$save");
            Assert.IsTrue(requestAction.Arguments[9] == "--p1");
            Assert.IsTrue(requestAction.Arguments[10] == "a");
        }

        [TestMethod]
        public void TestPrefixedRequestActionNotExistsButWithoutPrefix()
        {
            this.InitializeApp(@"save-not-exists --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a", null, '$');
            var request = App.Current.Request;
            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == null);
            Assert.IsTrue(request.RequestActions.Count == 1);
            Assert.IsTrue(requestAction.Name == null);
            Assert.IsTrue(requestAction.Arguments[0] == @"save-not-exists");
            Assert.IsTrue(requestAction.Arguments[1] == "--p1");
            Assert.IsTrue(requestAction.Arguments[2] == "a");
            Assert.IsTrue(requestAction.Arguments[3] == "--p2");
            Assert.IsTrue(requestAction.Arguments[4] == "b");
            Assert.IsTrue(requestAction.Arguments[5] == "$save");
            Assert.IsTrue(requestAction.Arguments[6] == "--p1");
            Assert.IsTrue(requestAction.Arguments[7] == "a");
            Assert.IsTrue(requestAction.Arguments[8] == @"\$save");
            Assert.IsTrue(requestAction.Arguments[9] == "--p1");
            Assert.IsTrue(requestAction.Arguments[10] == "a");
        }

        [TestMethod]
        public void TestPrefixedRequestActionExists()
        {
            this.InitializeApp(@"$save --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a \$save-not-exists --p1 a $save-not-exists $delete --p1", null, '$');
            var request = App.Current.Request;
            
            Assert.IsTrue(request.RequestActions.Count == 3);

            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == "save");
            Assert.IsTrue(requestAction.Name == "save");
            var i = 0;
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
            Assert.IsTrue(requestAction.Arguments[i++] == "a");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p2");
            Assert.IsTrue(requestAction.Arguments[i++] == "b");

            requestAction = request.RequestActions.LastOrDefault(f => f.Name == "save");
            Assert.IsTrue(requestAction.Name == "save");
            i = 0;
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
            Assert.IsTrue(requestAction.Arguments[i++] == "a");
            Assert.IsTrue(requestAction.Arguments[i++] == "$save");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
            Assert.IsTrue(requestAction.Arguments[i++] == "a");
            Assert.IsTrue(requestAction.Arguments[i++] == @"\$save-not-exists");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
            Assert.IsTrue(requestAction.Arguments[i++] == "a");
            Assert.IsTrue(requestAction.Arguments[i++] == "$save-not-exists");

            requestAction = request.RequestActions.LastOrDefault(f => f.Name == "delete");
            Assert.IsTrue(requestAction.Name == "delete");
            i = 0;
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
        }

        [TestMethod]
        public void TestPrefixedRequestActionExistsButScaped()
        {
            this.InitializeApp(@"\$save --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a \$save-not-exists --p1 a $save-not-exists $delete --p1", null, '$');
            var request = App.Current.Request;

            Assert.IsTrue(request.RequestActions.Count == 1);

            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == null);
            Assert.IsTrue(requestAction.Name == null);
            var i = 0;
            Assert.IsTrue(requestAction.Arguments[i++] == "$save");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
            Assert.IsTrue(requestAction.Arguments[i++] == "a");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p2");
            Assert.IsTrue(requestAction.Arguments[i++] == "b");
            Assert.IsTrue(requestAction.Arguments[i++] == "$save");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
            Assert.IsTrue(requestAction.Arguments[i++] == "a");
            Assert.IsTrue(requestAction.Arguments[i++] == @"\$save");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
            Assert.IsTrue(requestAction.Arguments[i++] == "a");
            Assert.IsTrue(requestAction.Arguments[i++] == @"\$save-not-exists");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
            Assert.IsTrue(requestAction.Arguments[i++] == "a");
            Assert.IsTrue(requestAction.Arguments[i++] == "$save-not-exists");
            Assert.IsTrue(requestAction.Arguments[i++] == "$delete");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
        }

        [TestMethod]
        public void TestPrefixedRequestActionNotExists()
        {
            this.InitializeApp(@"$save-not-exists --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a \$save-not-exists --p1 a $save-not-exists $delete --p1", null, '$');
            var request = App.Current.Request;

            Assert.IsTrue(request.RequestActions.Count == 1);

            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == null);
            Assert.IsTrue(requestAction.Name == null);
            var i = 0;
            Assert.IsTrue(requestAction.Arguments[i++] == "$save-not-exists");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
            Assert.IsTrue(requestAction.Arguments[i++] == "a");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p2");
            Assert.IsTrue(requestAction.Arguments[i++] == "b");
            Assert.IsTrue(requestAction.Arguments[i++] == "$save");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
            Assert.IsTrue(requestAction.Arguments[i++] == "a");
            Assert.IsTrue(requestAction.Arguments[i++] == @"\$save");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
            Assert.IsTrue(requestAction.Arguments[i++] == "a");
            Assert.IsTrue(requestAction.Arguments[i++] == @"\$save-not-exists");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
            Assert.IsTrue(requestAction.Arguments[i++] == "a");
            Assert.IsTrue(requestAction.Arguments[i++] == "$save-not-exists");
            Assert.IsTrue(requestAction.Arguments[i++] == "$delete");
            Assert.IsTrue(requestAction.Arguments[i++] == "--p1");
        }

        [TestMethod]
        public void TestPrefixedRequestActionNotExistsAndScaped()
        {
            this.InitializeApp(@"\$save-not-exists --p1 ""a"" --p2 ""b"" $save --p1 a \$save --p1 a \$save-not-exists --p1 a $save-not-exists $delete --p1", null, '$');
            var request = App.Current.Request;

            Assert.IsTrue(request.RequestActions.Count == 1);

            var requestAction = request.RequestActions.FirstOrDefault(f => f.Name == null);
            Assert.IsTrue(requestAction.Name == null);
            var i = 0;
            Assert.IsTrue(requestAction.Arguments[i++] == @"\$save-not-exists");
        }
        #endregion
    }
}

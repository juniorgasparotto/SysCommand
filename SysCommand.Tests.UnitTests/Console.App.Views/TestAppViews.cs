using Microsoft.VisualStudio.TestTools.UnitTesting;
using SysCommand.Test;
using System.Text;
using SysCommand.ConsoleApp.View;
using SysCommand.DefaultExecutor;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestAppViews
    {
        public TestAppViews()
        {
            TestHelper.SetCultureInfoToInvariant();
        }

        [TestMethod]
        public void TestTableViewWith4ColumnsAndChunkText()
        {
            var strBuilder = new StringBuilder();
            var table = new TableView(strBuilder);
            table.AddLineSeparator = true;
            table.AddColumnSeparator = true;

            table.PaddingLeft = 2;
            table.PaddingTop = 1;
            table.PaddingBottom = 2;
            table.IncludeHeader = true;
            table.AddColumnDefinition("A", 10, 0, 2);
            table.AddColumnDefinition("B", 10);
            table.AddColumnDefinition("C", 10);
            table.AddColumnDefinition("D", 10);

            table.AddRow()
                .AddColumnInRow("0000000000000000000000")
                .AddColumnInRow("111111111111 11111111111111 1111")
                .AddColumnInRow("22222222222 222222222 222222222 2222222")
                .AddColumnInRow("33333333333333333333333333 333333333333 333 3 3 3333 3");

            table.AddRow()
                .AddColumnInRow("4444000000000")
                .AddColumnInRow("5555 11111111111111 1111")
                .AddColumnInRow("66666 222222222 222222222 2222222")
                .AddColumnInRow("777777 333333333333 333 3 3 3333 3");

            table.AddRow()
                .AddColumnInRow("88888888888800")
                .AddColumnInRow("AAAAAA AAAAAAAAA AAAAAAAAAAAA")
                .AddColumnInRow("CCCCCCCCC 222222222 222222222 2222222")
                .AddColumnInRow("It is a long established fact that a reader It is a long established fact that a reader");

            table.Build();
            this.Compare(table.ToString(), TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void TestTableViewForListObject()
        {
            var mapper = new ArgumentMapper();
            var maps = mapper.Map(new CommandTableView());
            var table = TableView.ToTableView<ArgumentMap>(maps);
            table.Build();
            this.Compare(table.ToString(), TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void TestTableViewOnlyOneSummaryChunkAndWithPaddingTopAndLeft()
        {
            var table = new TableView();
            table.AddLineSeparator = false;
            table.AddColumnSeparator = false;
            table.PaddingTop = 1;
            table.AddRowSummary("is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy ", 30, 30);
            table.Build();
            this.Compare(table.ToString(), TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void TestTableViewOnlyOneSummaryChunkAndWithPaddingTopAndLeftWithLines()
        {
            var table = new TableView();
            table.AddLineSeparator = true;
            table.AddColumnSeparator = true;
            table.PaddingTop = 1;
            table.AddRowSummary("is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy", 30, 30);
            table.Build();
            this.Compare(table.ToString(), TestHelper.GetCurrentMethodName());
        }

        [TestMethod]
        public void TestTableViewLikeGitHelp()
        {
            var strBuilder = new StringBuilder();
            var table = new TableView(strBuilder);
            table.AddLineSeparator = false;
            table.AddColumnSeparator = false;

            table.AddColumnDefinition(null, 44, 0, 4);
            table.AddColumnDefinition(null, 64);
            table.AddRow()
                .AddColumnInRow("usage: git")
                .AddColumnInRow("[--version] [--help] [-C <path>] [-c name=value] [--exec-path[=<path>]] [--html-path] [--man-path] [--info-path] [-p | --paginate | --no-pager] --no-replace-objects] [--bare] [--git-dir=<path>] [--work-tree=<path>] [--namespace=<name>] <command> [<args>]");
            table.Build();

            strBuilder.AppendLine();
            strBuilder.AppendLine();

            var table2 = new TableView(strBuilder);
            table2.AddLineSeparator = false;
            table2.AddColumnSeparator = false;
            table2.AddColumnDefinition(null, 0, 3, 4);
            table2.AddColumnDefinition(null, 0);

            // ***** 
            table2.AddRowSummary("These are common Git commands used in various situations:");
            table2.AddRowSummary("");

            // ***** 
            table2.AddRowSummary("start a working area (see also: git help tutorial)");
            table2.AddRow()
                .AddColumnInRow("clone")
                .AddColumnInRow("Clone a repository into a new directory");

            table2.AddRow()
                .AddColumnInRow("init")
                .AddColumnInRow("Create an empty Git repository or reinitialize an existing one");
            table2.AddRowSummary("");

            // *****
            table2.AddRowSummary("work on the current change (see also: git help everyday)");
            table2.AddRow()
                .AddColumnInRow("add")
                .AddColumnInRow("Add file contents to the index");

            table2.AddRow()
                .AddColumnInRow("mv")
                .AddColumnInRow("Move or rename a file, a directory, or a symlink");

            table2.AddRow()
                .AddColumnInRow("reset")
                .AddColumnInRow("Reset current HEAD to the specified state");

            table2.AddRow()
                .AddColumnInRow("rm")
                .AddColumnInRow("Remove files from the working tree and from the index");

            table2.AddRowSummary("");

            // *****
            table2.AddRowSummary("examine the history and state (see also: git help revisions)");

            table2.AddRow()
                .AddColumnInRow("bisect")
                .AddColumnInRow("Use binary search to find the commit that introduced a bug");

            table2.AddRow()
                .AddColumnInRow("grep")
                .AddColumnInRow("Print lines matching a pattern");

            table2.AddRow()
                .AddColumnInRow("log")
                .AddColumnInRow("Show commit logs");

            table2.AddRow()
               .AddColumnInRow("show")
               .AddColumnInRow("Show various types of objects");

            table2.AddRow()
               .AddColumnInRow("status")
               .AddColumnInRow("Show the working tree status");

            table2.AddRowSummary("");

            // *****
            table2.AddRowSummary("grow, mark and tweak your common history");

            table2.AddRow()
                .AddColumnInRow("branch")
                .AddColumnInRow("List, create, or delete branches");

            table2.AddRow()
               .AddColumnInRow("checkout")
               .AddColumnInRow("Switch branches or restore working tree files");

            table2.AddRow()
               .AddColumnInRow("commit")
               .AddColumnInRow("Record changes to the repository");

            table2.AddRow()
               .AddColumnInRow("diff")
               .AddColumnInRow("Show changes between commits, commit and working tree, etc");

            table2.AddRow()
               .AddColumnInRow("merge")
               .AddColumnInRow("Join two or more development histories together");

            table2.AddRow()
               .AddColumnInRow("rebase")
               .AddColumnInRow("Reapply commits on top of another base tip");

            table2.AddRow()
               .AddColumnInRow("tag")
               .AddColumnInRow("Create, list, delete or verify a tag object signed with GPG");
            table2.AddRowSummary("");

            // *****
            table2.AddRowSummary("collaborate (see also: git help workflows)");

            table2.AddRow()
                .AddColumnInRow("fetch")
                .AddColumnInRow("Download objects and refs from another repository");

            table2.AddRow()
               .AddColumnInRow("pull")
               .AddColumnInRow("Fetch from and integrate with another repository or a local branch");

            table2.AddRow()
               .AddColumnInRow("push")
               .AddColumnInRow("Update remote refs along with associated objects");
            table2.Build();
            this.Compare(strBuilder.ToString(), TestHelper.GetCurrentMethodName());
        }

        private void Compare(string content, string funcName)
        {
            Assert.IsTrue(TestHelper.CompareObjects<TestAppViews>(content, null, funcName));
        }
    }
}

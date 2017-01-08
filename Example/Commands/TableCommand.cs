using System.Collections.Generic;
using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.View;

namespace Example.Commands
{
    public class TableCommand : Command
    {
        public string MyTable()
        {
            var list = new List<MyModel>
            {
                new MyModel() {Id = "1", Column2 = "Line 1 Line 1"},
                new MyModel() {Id = "2 " , Column2 = "Line 2 Line 2"},
                new MyModel() {Id = "3", Column2 = "Line 3 Line 3"}
            };

            return TableView.ToTableView(list)
                            .Build()
                            .ToString();
        }

        public class MyModel
        {
            public string Id { get; set; }
            public string Column2 { get; set; }
        }
    }

}

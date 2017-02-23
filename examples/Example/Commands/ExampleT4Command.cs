using Example.Views.ExampleT4;
using SysCommand.ConsoleApp;

namespace Example.Commands
{
    public class ExampleT4Command : Command
    {
        public string T4MyAction()
        {
            return ViewT4<MyActionView>();
        }

        public string T4MyAction2()
        {
            var model = new MyModel
            {
                Name = "MyName"
            };

            return ViewT4<MyActionView, MyModel>(model);
        }

        public class MyModel
        {
            public string Name { get; set; }
        }
    }
}

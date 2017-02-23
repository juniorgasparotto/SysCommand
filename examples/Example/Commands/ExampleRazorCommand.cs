using SysCommand.ConsoleApp;

namespace Example.Commands
{
    public class ExampleRazorCommand : Command
    {
        public string MyAction()
        {
            return View<MyModel>();
        }

        public string MyAction2()
        {
            var model = new MyModel
            {
                Name = "MyName"
            };

            return View(model, "MyAction.razor");
        }

        public class MyModel
        {
            public string Name { get; set; }
        }
    }
}

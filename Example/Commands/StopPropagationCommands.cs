using SysCommand.ConsoleApp;

namespace Example.Commands
{
    public class StopPropagationCommand1 : Command
    {
        public string StopPropagationAction1(bool cancel = false)
        {
            return "StopPropagationCommand1.StopPropagationAction1";
        }

        public string StopPropagationAction2()
        {
            return "StopPropagationCommand1.StopPropagationAction2";
        }
    }

    public class StopPropagationCommand2 : Command
    {
        public string StopPropagationAction1(bool cancel = false)
        {
            if (cancel)
            {
                ExecutionScope.StopPropagation();
            }

            return "StopPropagationCommand2.StopPropagationAction1";
        }

        public string StopPropagationAction2()
        {
            return "StopPropagationCommand2.StopPropagationAction2";
        }
    }

    public class StopPropagationCommand3 : Command
    {
        public string StopPropagationAction1(bool cancel = false)
        {
            return "StopPropagationCommand3.StopPropagationAction1";
        }

        public string StopPropagationAction2()
        {
            return "StopPropagationCommand3.StopPropagationAction2";
        }
    }
}
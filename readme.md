#SysCommand
Framework for development applications using the MVC pattern.

#Install

* NuGet: 
* NuGet Core CRL: 

#Simple example

```csharp

using SysCommand.ConsoleApp;

namespace Example
{
    public class Program
    {
        public static int Main()
        {
            return App.RunInfiniteIfDebug();
        }
    }

    public class HelloWorldWithPropertiesCommand : Command
    {
        public string Arg0 { get; set; }
        public string Arg1 { get; set; }

        public string Main()
        {
            return $"My console app like MVC (Arg0: {Arg0}; Arg1: {Arg1})";
        }
    }

    public class HelloWorldWithMethodsCommand : Command
    {
        public string HelloWorld(string myArg0, double? myArg1 = null)
        {
            return $"My HelloWorld (Arg0: {myArg0}; Arg1: {myArg1})";
        }
    }
}


```

##Features

  * Console Application with MVC
    * Razor templates: Just use the return "Command.View()" in your actions, like MVC Web application. (using System.Web.Razor dependency)
    * T4 templates: Just use the return "Command.ViewT4()" in your actions.
    * Indented text using the class "TableView".
    * Functionality Multi Action to be possible invoke several actions in the same input. By default is enable 'App.EnableMultiAction'.
  * Automatic configuration. Just the class inherit from "Command".
  * Automatic help functionality including usage mode. Just use the input actions 'help'
  * Functionality for saving command histories. Just use the input actions 'history-save [name]', 'history-load [name]', 'history-remove [name]' and 'history-list'
  * Simple mechanism of object persistence in JSON text files (using NewtonSoft dependency)
  * Mechanism to speed development in debug mode. Just use the "App.RunInfiniteIfDebug()" method.
    * Include the command 'clear' to clear the console window when in debug mode.
  * Mechanism to help write and read informations: Just use the console wrapper "App.Console":
    * Write: Print texts using the following verbs: "Info", "Success", "Warning", "Critical", "Error", "None", "All".
    * Read: If you use the 'Writes' methods is recommended use the reads methods.
    * Verbose: Choose which are verbs can be printed in console. Just use the input argument '-v' or '--verbose'
  * Functionality to persists anything in App scope (in memory). Just use 'App.Items".
  * Events controllers "OnComplete", "OnException" e etc...
  * Extras: Simple command line parser using "OptionSet" class.
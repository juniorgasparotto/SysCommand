#SysCommand
Framework for development applications using the MVC pattern.

#Install

* NuGet: 
* NuGet Core CRL: 

#Simple example

######Code

```csharp

namespace Example
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main()
        {
            return App.RunApplication();
        }
    }

    public class HelloWorld1 : Command
    {
        public string HelloWorld(string myArg0, int? myArg1 = null)
        {
            return string.Format("My HelloWorld (Arg0: {0}; Arg1: {1})", myArg0, myArg1);
        }
    }

    public class HelloWorld2 : Command
    {
        public string MyArg0 { get; set; }
        public string MyArg1 { get; set; }

        public string Main()
        {
            return string.Format("My console app like MVC  (Arg0: {0}; Arg1: {1})", MyArg0, MyArg1);
        }
    }
}

```

######Testing

```
C:\MyApp.exe hello-world --my-arg0 ABC
My HelloWorld (Arg0: ABC; Arg1: )

C:\MyApp.exe hello-world --my-arg0 ABC --my-arg1 10000
My HelloWorld (Arg0: ABC; Arg1: 10000)

C:\MyApp.exe --my-arg0 ABC
My console app like MVC  (Arg0: ABC; Arg1: )

C:\MyApp.exe --my-arg0 ABC --my-arg1 DEF
My console app like MVC  (Arg0: ABC; Arg1: DEF)
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
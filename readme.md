[
![Inglês](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/img/en-us.png)
](https://github.com/juniorgasparotto/SysCommand)
[
![Português](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/img/pt-br.png)
](https://github.com/juniorgasparotto/SysCommand/blob/master/readme-pt-br.md)

# <a name="presentation" />SysCommand

The `SysCommand` is a powerful cross-platform framework, to develop `Console Applications` in .NET. Is simple, type-safe, and with great influences of the MVC pattern.

## Build Status

<table>
    <tr><th>netstandard 1.6</th> <th>net45 +</th></tr>
    <tr>
        <td>

[![Build status](https://ci.appveyor.com/api/projects/status/6hb2sox6y6g5pwmt/branch/master?svg=true)](https://ci.appveyor.com/project/ThiagoSanches/syscommand-bg4ki/branch/master)
</td>
<td>

[![Build status](https://ci.appveyor.com/api/projects/status/36vajwj2n93f4u21/branch/master?svg=true)](https://ci.appveyor.com/project/ThiagoSanches/syscommand/branch/master)
</td>
</tr>

</table>

## Channels

* [Report an error](https://github.com/juniorgasparotto/SysCommand/issues/new)
* [Send a message](https://syscommand.slack.com/)

# <a name="install" />Installation

Via [NuGet](https://www.nuget.org/packages/SysCommand/):

```
Install-Package SysCommand
```

_Note: the package brings the `Program.cs.txt` file that contains a basic template. To use it, replace the contents of the `Program.cs` file with the contents of this file._

## <a name="presentation-how-it-works" />How does it work?

It works like an automated command-line parser, allowing the programmer to focus on the business rules of your application.

To do this, you can choose 3 ways of working:

* **Main-typed**method: this is equivalent to the traditional model `Main(string[] args)` , but typed.
* **Properties**: each property will be transformed into arguments.
* **Methods**: each method will be transformed into a sub-command: **Action**

In addition, he has a feature to simulate a command prompt within the own Visual Studio, eliminating the need to test your application outside of the development environment.

Other essential resources as `help` , `verbose` , `error handling` and others are also supported.

**Example of _Main type_:**

```csharp
namespace Example.Initialization.Simple
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();
        }
    }

    // Classes inheriting from `Command` will be automatically found by the system
    // and its public properties and methods will be available for use.
    public class MyCommand : Command
    {
        // This signature "Main(...)" is reserved to process arguments fastly.
        public void Main(string myArgument, int? myArgument2 = null)
        {
            // this arg is obrigatory
            this.App.Console.Write(string.Format("myArgument='{0}'", myArgument));

            // verify if property was inputed by user.
            if (myArgument2 != null)
                this.App.Console.Write(string.Format("myArgument2='{0}'", myArgument2));
        }
    }
}
```

_Tests at a command prompt:_

```
C:\Users\MyUser> MyApp.exe help
... the automatic help text will be shown ...

C:\Users\MyUser> MyApp.exe --my-argument "value"
myArgument='value'

C:\Users\MyUser> MyApp.exe --my-argument "value" --my-argument2 1000
myArgument='value'
myArgument2='1000'
```

_Tests in Visual Studio using the Simulator from console:_

```
cmd> help
... the automatic help text will be shown ...

cmd> --my-argument "value"
myArgument='value'

cmd> --my-argument "value" --my-argument2 1000
myArgument='value'
myArgument2='1000'
```

**Example of use with properties:**

```csharp
namespace Example.Initialization.Simple
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();
        }
    }

    public class MyCommand : Command
    {
        public string MyArgument { get; set; }

        // This signature "Main()" is reserved to process properties.
        public void Main()
        {
            if (MyArgument != null)
                this.App.Console.Write(string.Format("Main MyArgument='{0}'", MyArgument));
        }
    }
}
```

```
cmd> --my-argument value
Main MyArgument='value'
```

**Example of actions:**

```csharp
namespace Example.Initialization.Simple
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();
        }
    }

    public class MyCommand : Command
    {
        public void MyAction(bool a)
        {
            this.App.Console.Write(string.Format("MyAction a='{0}'", a));
        }
    }
}
```

```
cmd> my-action -a
MyAction a='True'
```

**_Note that there is no analysis code in any example, your code is clean and ready to receive commands._**

### Understand better ...

Technically, there are four entities that are the basis of the framework:

**`App`**

Is the application context, where a `App` contains several `Commands` . Is represented by the class `SysCommand.ConsoleApp.App` and must be the first entity to be configured in your `Main(string[] args)` method.

The application context initialization can be done in two ways, by an instance of the class `App` or static method `App.RunApplication` that provides a console simulation feature that helps you test your inputs inside the Visual Studio itself, without the need to perform your ".exe" in an external console, just press the _Play_. Learn more: [Starting](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#class-app) , [Booting with the console Simulator](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#initializing-by-static-method).

**`Command`**

The commands represent a grouping of features the same business context, similar to _MVC Controllers_. Programmatically they are represented by classes that inherit from `SysCommand.ConsoleApp.Command` . Each `Command` instance will have access to the current context by the property `this.App` .

By default, the system attempts to find automatically, any class that extend to `Command` , therefore it is not necessary to specify them in the boot record, although this is possible. Learn more: [Types of commands](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#kind-of-commands) , [Specifying the types of commands](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#specifying-commands).

**`Argument`**

The arguments represent the most basic of a console application, are known `--argument-name value` , `-v` and etc. Programmatically they are represented by _Properties_ of `Command` and shall be accompanied by a method called `Main()` (without parameters) to be able to intercept if a property was used. The name `Main` was chosen by the similarity of concept with the method `Main(string[] args)` .

User-side, no special syntax was created, known standards were implemented. The long arguments are accessed with the prefix `--` and are accompanied by the name of the argument. The short arguments are accessed with a dash `-` or a `/` bar and are accompanied by only one character. The values of the arguments must be in front of the argument name separated by a space ` ` or `:` or `=` . Positional inputs are also supported, allowing the omission of the name of the argument.

By default, all public properties of your `Command` are enabled to be `arguments` . Saiba mais: [Working with properties](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#properties), [Manual choice of properties via attribute](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#properties-ignore-public), [Input](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#input), [Supported types](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#support-types).

**`Action`**

Represent the same actions the _Actions of MVC Controllers_. Programmatically represent the _methods_ of `Command` and its parameters (if any) will be converted into `arguments` and that can only be accessed when accompanied by the name of the action.

Its use is similar to the way we use `git` resources like: `git add -A` ; `git commit -m "comments"` , where `add` and `commit` would be the name of the stock and `-A` , `-m` their respective arguments.

It is still possible to omit the name of the action in the user input. This feature is called the **default method** and looks very similar to the use of properties.

By default, all public methods of your `Command` are enabled to be `actions` . Learn more: [Working with methods](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#methods), [Ignore public methods by a manual choice using attribute](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#methods-ignore-public), [Standard methods](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#methods-default).

**Advanced example:**

```csharp
namespace Example.Initialization.Advanced
{
    using SysCommand.ConsoleApp;
    using SysCommand.Mapping;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication();

            // OR without "simulate console"
            // var myApp = new App();
            // myApp.Run(args);
            // return myApp.Console.ExitCode;
        }
    }

    public class GitCommand : Command
    {
        // usage:
        // MyApp.exe add --all
        public void Add(bool all)
        {
            this.App.Console.Error("Add error");
            this.App.Console.Write("Add");
        }

        // usage:
        // MyApp.exe commit -m "comments"
        public void Commit(string m)
        {
            this.App.Console.Error("Commit error");
            this.App.Console.Write("Commit");
        }
    }

    public class MyCommand : Command
    {
        // "Argument without customization"
        // usage:
        // MyApp.exe --my-property value
        public string MyProperty { get; set; }

        // "Argument customized"
        // usage:
        // MyApp.exe --custom-property 123
        // MyApp.exe -p 123
        [Argument(LongName = "custom-property", ShortName = 'p', Help = "My custom argument ")]
        public decimal? MyPropertyDecimal { get; set; }

        // Method to process arguments/properties, if any exist.
        // This signature "Main()" is reserved for this use only.
        public string Main()
        {
            if (MyProperty != null)
                this.App.Console.Write(string.Format("Main MyProperty='{0}'", MyProperty));

            if (MyPropertyDecimal != null)
                this.App.Console.Write(string.Format("Main MyPropertyDecimal='{0}'", MyPropertyDecimal));

            return "Return methods can also be used as output";
        }

        // "Action without customization"
        // usage:
        // MyApp.exe my-action -p value
        public string MyAction(string p)
        {
            // Example showing that properties are executed before methods
            if (MyPropertyDecimal != null)
                this.App.Console.Write("Use property here if you want!");

            return string.Format("MyAction p='{0}'", p);
        }

        // "Action without customization and is a overload"
        // usage:
        // MyApp.exe my-action -p value --p2
        public string MyAction(string p, bool p2)
        {
            return string.Format("MyAction p='{0}'; p2='{1}'", p, p2);
        }

        // "Action customized"
        // usage:
        // MyApp.exe custom-action
        // MyApp.exe custom-action -o
        [Action(Name = "custom-action", Help = "My custom action")]
        public string CustomAction
        (
            [Argument(ShortName = 'o')]
            bool? optionalParameter = null
        )
        {
            return string.Format("MyCustomAction optionalParameter='{0}'", optionalParameter);
        }
    }
}
```

_Input to display the help:_

```
cmd> help
... show help here ...
```

_Inputs similar to using git:_

```
cmd> add --all
Add

cmd> commit -m "comments"
Commit
```

_Inputs with 3 kinds of value separators:_

```
cmd> --my-property value
Main MyProperty='value'
Return methods can also be used as output

cmd> --my-property=value
Main MyProperty='value'
Return methods can also be used as output

cmd> --custom-property:123
Main MyPropertyDecimal='123'
Return methods can also be used as output
```

_Inputs with 2 enclosing types of arguments in short form:_

```
cmd> -p 123
Main MyPropertyDecimal='123'
Return methods can also be used as output

cmd> /p 123
Main MyPropertyDecimal='123'
Return methods can also be used as output
```

_Inputs with 2 method overloads: MyAction_

```
cmd> my-action -p value
MyAction p='value'

cmd> my-action -p value --p2
MyAction p='value'; p2='True'
```

_Positional inputs:_

```
cmd> my-action positional-value
MyAction p='positional-value'

cmd> my-action positional-value false
MyAction p='positional-value'; p2='False'
```

_Inputs with optional parameters:_

```
cmd> custom-action
MyCustomAction optionalParameter=''

cmd> custom-action -o
MyCustomAction optionalParameter='True'
```

_Input with different commands and arguments with the `--verbose` argument to allow show errors:_

```
cmd> commit -m "my commit" --my-property=value --custom-property:123 --verbose Error
Main MyProperty='value'
Main MyPropertyDecimal='123'
Return methods can also be used as output
Commit error
Commit
```

**Learn more ...**

* Note, the primitive types for each property are configured as `Nullable` . It is important to be able to identify that the user entered a particular property. Learn more: [Working with properties](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#properties).
* All primitive types of .NET, Enums, Enums and Flags Collections are supported. See the [Supported types](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#support-types)topic.
* Use `App.Console.Write()` , `App.Console.Error()` (among others) to print their outputs and enjoy features like the `verbose` . Learn more: [Verbose](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#verbose).
* You can use the return of methods like `output` , including the reserved method `Main()` . Or use `void` If you do not want to use this feature. Learn more: [Output](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#output).
* If you want, customize your `arguments` or `actions` using the attributes `ArgumentAttribute` and `ActionAttribute` . You can customize several attributes such as names, help text and in others. Learn more: [Customizing the names of the arguments](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#properties-customizing-name) and [Customizing actions names and arguments](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#methods-customizing-names).
* You can use methods with the same name (overloads) to define different `actions` . They can be invoked from the command prompt with the same name, but the arguments define which method to invoke, the same occurs in `c#` . Learn more:[Overloads](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#methods-overloads)
* Choose to use the `int Program.Main(string[] args)` return method, so you can return the status code for the console. (ERROR = 1 or SUCCESS = 0).
* There is also native support to generate help text. Learn more: [Help](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#help).

This was just a summary, for more on this see our project [Documentation](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#documentation).

## <a name="what-is-the-purpose" />What is the purpose of this project?

The goal is to help developers in any programming language that suffer to create a console application due to bureaucracy of the parse and difficulty of maintenance. If you are like me who loves to create gadgets to solve everyday problems using consoles, so join us!

If you have never worked with .NET, maybe this is an excellent opportunity to meet him. With the new .NET (Core Clr) you can create applications on any platform and it can be simplified with the `SysCommand` .

# <a name="install-dlls" />Package DLLs

* `SysCommand.dll`: Contains all the logic to parse and execute command lines. Everything was thought to make the MVC pattern as natural as possible.
* `NewtonSoft.Json`and `System.Web.Razor` : Are required dependencies in some features that are explained in the documentation.

## <a name="install-step-a-step" />Step by step how to use

* Create your project`Console Application`
* Install `SysCommand` in your project`Console Application`
* In the first line of the method `public int Program.Main(string[] args)` , add the code `return App.RunApplication()` .
* Create a class, anywhere, that inherits from `SysCommand.ConsoleApp.Command` .
* Create their properties with their types `Nullable` and let them as public. They will become `arguments` at the command prompt.
* Create a method `Main()` without parameters in your class to be able to intercept the inputs of its properties. Use `Property != null` to identify the property was inserted.
* Create public methods, with or without parameters, so they become `actions` . If you have optional parameters, let them set to `Nullable` for the same reason above.
* Type `help` in the command prompt that opens so you can view its properties and methods into `arguments` and `actions` .
* Now just use!

# Documentation

See the full documentation by clicking [here](https://github.com/juniorgasparotto/SysCommand/blob/master/documentation/en.md#documentation)

# <a name="license" />License

The MIT License (MIT)

Copyright (c) 2017 Glauber Donizeti Gasparotto Junior

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so , subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN THE EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

* * *

<sub>This text was translated by a machine</sub>

https://github.com/juniorgasparotto/MarkdownGenerator
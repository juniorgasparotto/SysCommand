
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

    // example with method
    public class Command1 : Command
    {
        // overload1
        public string HelloWorld(string myArg0, int? myArg1 = null)
        {
            return string.Format("My HelloWorld1 (Arg0: {0}; Arg1: {1})", myArg0, myArg1);
        }

        // overload2
        public string HelloWorld(string myArg0, DateTime myArg1)
        {
            return string.Format("My HelloWorld2 (Arg0: {0}; Arg1: {1})", myArg0, myArg1);
        }
    }
    
    // example with properties
    public class Command2 : Command
    {
        public string MyArg0 { get; set; }
        public string MyArg1 { get; set; }

        // the method main() without params is used (by convention) when one or more args is parsed
        public string Main()
        {
            return string.Format("My HelloWorld3 (Arg0: {0}; Arg1: {1})", MyArg0, MyArg1);
        }
    }
}
```
######Tests and Results

```
C:\MyApp.exe hello-world --my-arg0 ABC
My HelloWorld1 (Arg0: ABC; Arg1: )

C:\MyApp.exe hello-world ABC
My HelloWorld1 (Arg0: ABC; Arg1: ) // positional

C:\MyApp.exe hello-world --my-arg0 ABC --my-arg1 10000
My HelloWorld1 (Arg0: ABC; Arg1: 10000)

C:\MyApp.exe hello-world ABC 10000
My HelloWorld1 (Arg0: ABC; Arg1: 10000) // positional

C:\MyApp.exe hello-world --my-arg0 ABC --my-arg1 "2017-01-01 10:10:22"
My HelloWorld2 (Arg0: ABC; Arg1: 01/01/2017 10:10:22)

C:\MyApp.exe hello-world ABC "2017-01-01 10:10:22"
My HelloWorld2 (Arg0: ABC; Arg1: 01/01/2017 10:10:22) // positional

C:\MyApp.exe --my-arg0 ABC
My HelloWorld3 (Arg0: ABC; Arg1: )

C:\MyApp.exe --my-arg0 ABC --my-arg1 DEF
My HelloWorld3 (Arg0: ABC; Arg1: DEF)
```

##Comportamento padrão

Ao criar-se uma classe que herda de `Command`, em qualquer lugar do seu projeto console application, todos os seus métodos e propriedades `publicas` serão habilitados para serem chamados via prompt de comando automaticamente.

Os métodos serão convertidos em ações e as propriedades em argumentos de acordo com a seguinte regra: Converte o nome do membro (métodos, parametros e propriedades) em minusculo e adiciona um traço "-" antes de cada letra maiuscula que estiver no meio ou no final do nome. 

**Exemplo:**

```csharp
public string MyProperty { get;set; }
public void MyAction(string MyArgument);
```

```MyApp.exe my-action --my-argument value1 --my-property value2```

Em caso de propriedades ou paramentros de métodos com apenas uma letra, o padrão será deixar a letra minuscula e o input será aceito apenas na forma curta.

**Exemplo:**

```csharp
public string S { get;set; }
public void MyAction(string A);
```

```MyApp.exe -s value1 my-action -a value2```

* Para ter mais controle de quais comandos podem ser utilizados, veja o tópico de `Inicialização`.
* Para desabilitar a disponibilidade automatica de membros `publicos` ou customizar os nomes dos membros, veja o tópico de `Customizações`.

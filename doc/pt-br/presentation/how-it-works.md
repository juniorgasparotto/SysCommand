## Como funciona? !heading

Ele funciona como um analisar de linhas de comando automático, ou seja, toda a tarefa de parse fica por conta do framework, deixando o programador focado nas regras de negócios de sua aplicação.

**Exemplo:**

```csharp
public class Program
{
    public static int Main(string[] args)
    {
        return App.RunApplication();
    }
}

public class MyCommand : Command
{
    public int? MyProperty { get; set; }
    public decimal? MyProperty2 { get; set; }

    public void Main()
    {
        if (MyProperty != null)
            Console.WriteLine("MyProperty");
        if (MyProperty2 != null)
            Console.WriteLine("MyProperty2");
    }

    public string MyAction(string myParameter)
    {
        return "MyAction";
    }
}
```

output

```
MyApp.exe --property 123
MyProperty

MyApp.exe --property2 123.99
MyProperty2

MyApp.exe my-action --my-parameter value
MyAction

MyApp.exe my-action --my-parameter value --property 123 --property2 0.1
MyAction
MyProperty
MyProperty2
```

* Todos os tipos primitivos do .NET, Enums e Collections são suportados. Veja o tópico de [Tipos suportados](#tipos-suportados)

    Supported Syntax
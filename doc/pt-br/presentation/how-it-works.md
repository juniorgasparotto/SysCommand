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
    // Argument1
    public int? MyProperty { get; set; }
    // Argument2..You can create as many as you want.
    public decimal? MyProperty2 { get; set; }

    // Method to process arguments if any exist.
    // This signature "Main()" is reserved for this use only.
    public void Main()
    {
        if (MyProperty != null)
            Console.WriteLine("MyProperty");
        if (MyProperty2 != null)
            Console.WriteLine("MyProperty2");
    }

    // Action to do something... you can create as many as you want.
    public string MyAction(string myParameter)
    {
        if (MyProperty != null)
            Console.WriteLine("Use property here if you want!");
        return "MyAction";
    }
}
```

output

```
cmd> MyApp.exe --property 123
MyProperty

cmd> MyApp.exe --property2=123.99
MyProperty2

cmd> MyApp.exe my-action --my-parameter value
MyAction

cmd> MyApp.exe my-action --my-parameter value /property:123 --property2 0.1
MyAction
Use property here if you want!
MyProperty
MyProperty2
```

* Os tipos de input se dividem entre `arguments` (representados por propriedades) ou `actions` (representados por métodos). Veja [Trabalhando com propriedades](#trabalhando-com-propriedades) e [Trabalhando com métodos](#trabalhando-com-métodos)
* Existe suporte nativo para o `help` e `verbose`. Veja [Help automatico](#help-automatico) e [Verbose](#verbose)
* A sintaxe dos inputs se baseia no modelo: `[action-name ][-|--|/][argument-name][=|:| ][value]`. O caracter "|" significa "ou"
  * A forma curta é representada apenas por apenas um traço `-`
  * A forma longa pode ser representada por dois traços `--` ou pelo caracter `/`
  * O valor do argumento deve vir depois de um espaço ` ` ou unido com o nome do argumento por um `=` ou `:`
  * Todos os tipos primitivos do .NET, Enums e Collections são suportados.
  * Veja o tópico de [Tipos suportados](#tipos-suportados)
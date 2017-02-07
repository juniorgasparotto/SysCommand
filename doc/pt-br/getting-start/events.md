## Controle de eventos <header-set anchor-name="events" />

Os eventos são importantes para interceptar cada passo da execução e modificar ou extender o comportamento padrão. Os eventos existentes são os seguintes:

* `App.OnBeforeMemberInvoke(ApplicationResult, IMemberResult)`: Chamado antes do invoke de cada membro (propriedade ou metodo) que foi parseado.
* `App.OnAfterMemberInvoke(ApplicationResult, IMemberResult)`: Chamado depois do invoke de cada membro (propriedade ou metodo) que foi parseado.
* `App.OnMethodReturn(ApplicationResult, IMemberResult)`: : Chamado sempre que um metodo retorna valor
* `App.OnComplete(ApplicationResult)`: Chamado ao fim da execução
* `App.OnException(ApplicationResult, Exception)`: Chamado em caso de exception.

**Exemplo:**

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var app = new App();
        
        app.OnBeforeMemberInvoke += (appResult, memberResult) =>
        {
            app.Console.Write("Before: " + memberResult.Name);
        };

        app.OnAfterMemberInvoke += (appResult, memberResult) =>
        {
            app.Console.Write("After: " + memberResult.Name);
        };

        app.OnMethodReturn += (appResult, memberResult) =>
        {
            app.Console.Write("After MethodReturn: " + memberResult.Name);
        };

        app.OnComplete += (appResult) =>
        {
            app.Console.Write("Count: " + appResult.ExecutionResult.Results.Count());
            throw new Exception("Some error!!!");
        };

        app.OnException += (appResult, exception) =>
        {
            app.Console.Write(exception.Message);
        };

        app.Run(args);
    }

    public class FirstCommand : Command
    {
        public string MyProperty { get; set; }

        public string MyAction()
        {
            return "Return MyAction";
        }
    }
}
```

```
MyApp.exe --my-property value my-action
Before: MyProperty
After: MyProperty
Before: MyAction
After: MyAction
Return MyAction
After MethodReturn: MyAction
Count: 2
Some error!!!
```

No exemplo acima o controle passou para quem implementou os eventos e cada um dos eventos foram executados em sua respectiva ordem. 

Por padrão nos inserimos um handler chamado `SysCommand.ConsoleApp.Handlers.DefaultApplicationHandler` que é responsável pelo mecanismo padrão de `outputs` e controles de `erros`. Esse handler foi o responsável imprimir a linha "Return MyAction" do output acima. Para desliga-lo e ter o controle total dos eventos, basta desabilitar a flag `addDefaultAppHandler = false` no construtor.

```csharp
new App(addDefaultAppHandler: false).Run(args);
```

Outro modo de adicionar eventos é usando a interface `SysCommand.ConsoleApp.Handlers.IApplicationHandler`. Dessa maneira sua regra fica isolada, mas tendo o contraponto de ser obrigado a implementar todos os métodos da interface. Para adicionar um novo handler siga o exemplo abaixo:

```csharp
new App(addDefaultAppHandler: false)
        .AddApplicationHandler(new CustomApplicationHandler())
        .Run(args);
```

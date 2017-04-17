# Variáveis de contexto <header-set anchor-name="variable" />

A propriedade `App.Items` é a responsável por manter um escopo isolado de variáveis por cada instância da classe `App`. Na prática ela é uma coleção de objetos (chave/valor) que pode auxiliar na passagem de valores entre os comandos.

Essa coleção herda de `Dictionary<object, object>` e foi estendida com a adição de alguns métodos de ajuda:

* `T Get<T>()`: Retorna o primeiro elemento do tipo `T`, se não encontrar então retorna `null` para tipos complexos e `Exception` para `struct`.
* `T Get<T>(object key)`: Retorna o primeiro elemento da chave informada, o comportamento de retorno é o mesmo do método acima.
* `T GetOrCreate<T>()`: Se existir, retorna o primeiro elemento do tipo `T` ou cria uma nova instância via reflexão onde o tipo `T` será a chave.
* `T GetOrCreate<T>(object key)`: Se existir, retorna o primeiro elemento da chave informada ou cria uma nova instância via reflexão.

_Nota: Para a criação de novas instâncias via reflexão é necessário que a classe tenha um construtor sem parâmetros._

**Exemplo:**

```csharp
namespace Example.ContextVariable
{
    using SysCommand.ConsoleApp;

    public class Program
    {
        public static int Main(string[] args)
        {
            return App.RunApplication(delegate()
            {
                var app = new App();
                app.Items["variable1"] = 1;
                return app;
            });
        }
    }

    public class Command1 : Command
    {
        public void Action()
        {
            this.App.Console.Write(App.Items["variable1"]);
            App.Items["variable1"] = (int)App.Items["variable1"] + 1;
        }

        public void Action2()
        {
            this.App.Console.Write(App.Items["variable1"]);
        }
    }
}
```

```
MyApp.exe action action2
1
2
```

Note que a variável `variable1` foi atribuída na criação do contexto `App` e foi incrementada quando passou na action `action2`.
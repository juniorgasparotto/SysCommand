# Gerenciamento de históricos de argumentos !heading

Esse recurso permite que você salve aqueles inputs que são utilizados com muita frequencia e podem ser persistidos indeterminadamente. O seu funcionamento é bem simples, uma `Command` interno chamado `SysCommand.ConsoleApp.Commands.ArgsHistoryCommand` é responsável por indentificar as `actions` de gerenciamento e persisti-lo em um arquivo `Json` no caminho padrão `.app/history.json`. As `actions` de gerenciamento são as seguintes:

* `history-save   [name]`
* `history-load   [name]`
* `history-delete [name]` 
* `history-list`

**Exemplo:**

```csharp
public class TestArgsHistories : Command
{
    public void TestHistoryAction()
    {
        this.App.Console.Write("Testing"); 
    }
}
```

```
C:\MyApp.exe test-history-action history-save "CommonCommand1"
Testing

C:\MyApp.exe history-load "CommonCommand1"
Testing

C:\MyApp.exe history-list
[CommonCommand1] test-history-action

C:\MyApp.exe history-remove "CommonCommand1"
{No output}

C:\MyApp.exe history-list
{No output}
```

* Para desativar o comando `ArgsHistoryCommand` veja o tópico de `Inicialização`.
* A action `history-load` retorna um objeto do tipo `RedirectResult` que força o redirecionamento para um novo comando. Qualquer input depois dessa action será desprezado. Veja o tópico `Redirecionamento de comandos`.
* Esse recurso só vai funcionar se a flag `App.EnableMultiAction` estiver ligada.
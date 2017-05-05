# Gerenciamento de históricos de argumentos <header-set anchor-name="argument-history-manager" />

Esse recurso permite que você salve aqueles inputs que são utilizados com muita frequencia e podem ser persistidos indeterminadamente. O seu funcionamento é bem simples, um `Command` interno chamado `SysCommand.ConsoleApp.Commands.ArgsHistoryCommand` é responsável por salvar os comandos e carrega-los quando solicitado. O arquivo `.app/history.json` é onde ficam salvos os comandos no formato `Json`. As `actions` de gerenciamento são as seguintes:

* `history-save   [name]`: Utilize para salvar um comando. É obrigatório especificar um nome.
* `history-load   [name]`: Utilize para carregar um comando usando um nome salvo anteriormente.
* `history-delete [name]`: Utilize para deletar um comando.
* `history-list`: Utilize para listar todos os comandos salvos.

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
C:\MyApp.exe history-list
```

Os dois últimos comandos não retornam outputs.

* Para desativar o comando `ArgsHistoryCommand` veja o tópico <anchor-get name="specifying-commands" />.
* A action `history-load` retorna um objeto do tipo `RedirectResult` que força o redirecionamento para um novo comando. Qualquer input depois dessa action será desprezado. Veja o tópico <anchor-get name="redirectiong-commands" />.
* Esse recurso só vai funcionar se a flag `App.EnableMultiAction` estiver ligada.
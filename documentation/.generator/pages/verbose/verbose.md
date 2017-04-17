# Verbose <header-set anchor-name="verbose" />

O controle de exibição por verbo esta contido em um comando interno chamado `SysCommand.ConsoleApp.Commands.VerboseCommand`. A sua função é alterar o valor da propriedade `App.Console.Verbose` caso o usuário envie um input de verbose. Atualmente, os verbos suportados são:

* `All`: Exibe todos os verbos
* `Info`: É o verbo padrão, sempre será exibido, ao menos que o usuário envie o verbo `Quiet`.
* `Success`: Verbo para mensagens de sucesso. Só será exibido se o usuário solicitar.
* `Critical`: Verbo para mensagens criticas. Só será exibido se o usuário solicitar.
* `Warning`: Verbo para mensagens de warning. Só será exibido se o usuário solicitar.
* `Error`: Verbo para mensagens de erro. O sistema força o envio desse verbo em caso de erros de parse. Só será exibido se o usuário solicitar.
* `Quiet`: Verbo para não exibir nenhuma mensagem, porém se a mensagem estiver sendo forçada, esse verbo é ignorado para essa mensagem.

Para que a funcionalidade funcione corretamente é obrigatorio o uso das funções de output contidas dentro da classe `SysCommand.ConsoleApp.ConsoleWrapper` e que tem uma instância disponível na propriedade `App.Console`.

**Exemplo:**

```csharp
public class TestVerbose : Command
{
    public void Test()
    {
        this.App.Console.Write("output of info"); 
        this.App.Console.Error("output of error");
        this.App.Console.Error("output of error forced", forceWrite: true);
        this.App.Console.Critical("output of critical");
    }
}
```

_Forma curta:_

```
MyApp.exe test -v Critical
```

_Forma longa:_

```
MyApp.exe test --verbose Critical
```

Outputs:

```
output of info
output of error forced
output of critical
```

É importante dizer que você pode desligar esse recurso e implementar seu próprio mecanismo de verbose. Para isso você precisa desativar o comando `VerboseCommand` e criar seu próprio conjunto de funções para cada verbo.

* Para desativar o comando `VerboseCommand` utilize a forma exclusiva de especificação de comandos. Veja o tópico <anchor-get name="specifying-commands" />.
# Verbose <header-set anchor-name="verbose" />

O controle de exibição por verbo esta contido em um comando interno chamado `SysCommand.ConsoleApp.Commands.VerboseCommand`. A sua função é alterar o valor da propriedade `App.Console.Verbose` caso o usuário envie um input de verbose. Atualmente, os verbos suportados são:

* `All`: Exibe todos os verbos
* `Info`: É o verbo padrão, sempre será exibido, ao menos que o usuário envie o verbo `Quiet`.
* `Success`: Verbo para mensagens de sucesso.
* `Critical`: Verbo para mensagens criticas.
* `Warning`: Verbo para mensagens de warning.
* `Error`: Verbo para mensagens de erro. O sistema força o envio desse verbo em caso de erros de parse.
* `Quiet`: Verbo para não exibir nenhuma mensagem, porém se a mensagem estiver sendo forçada, esse verbo é ignorado para essa mensagem.

Para que a funcionalidade funcione corretamente é obrigatorio o uso das funções de output contidas dentro da classe `ConsoleWrapper` e que tem uma instancia disponível na propriedade `App.Console`. 

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

Forma curta: ```MyApp.exe test -v Critical```

Forma longa: ```MyApp.exe test --verbose Critical```

Outputs:

```
output of info
output of error forced
output of critical
```

* Para desativar o comando `VerboseCommand` veja o tópico de <anchor-get name="specifying-commands" />.


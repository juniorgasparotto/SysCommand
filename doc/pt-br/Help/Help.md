# Help <header-set anchor-name="help" />

O formato do help leva em consideração todos os elementos que compõem o sistema, ou seja, `Commands`, `Arguments`  e `Actions`. Ele é gerado de forma automática utilizando os textos de help de cada um desses elementos, por isso é importante manter essas informações preenchidas e atualizadas, isso ajudará você e quem for utilizar sua aplicação. 

No formato padrão, existem duas formas de exibir o help: o `help completo` e o `help por action`:

**Exibe o help para uma ação especifica:**

```MyApp.exe help my-action-name```

**Exibe o help completo:**

```MyApp.exe help```

Para o `help completo`, o formato de saída que será exibido será o seguinte:

```
usage:    [--argument=<phrase>] [--argument-number=<number>]
          [-v, --verbose=<None|All|Info|Success|Critical|Warning|Error|Quiet>]
          --required-argument=<phase>
          <actions[args]> (A)

Command help (B)
    LongName (C1), ShortName (C2)      Help text for arguments of command (properties) (C3). Complement (C4)
    Action (D)
      LongName (E1), ShortName (E2)    Help text for arguments of actions (parameters) (E3). Complement (E4)

Use 'help --action=<name>' to view the details of
any action. Every action with the symbol "*" can
have his name omitted. (F)
```

A fonte de cada texto esta em cada elemento `Commands`, `Arguments`  e `Actions` e os textos complementares estão na classe estática `SysCommand.ConsoleApp.Strings`. Segue o mapeamento de cada texto conforme o formato exibido acima:

* **A:** O texto `usage` é gerado internamente pela classe `DefaultDescriptor` e sempre será exibido.
* **B:** O texto do `Command` sempre será exibido e a sua fonte vem da propriedade `Command.HelpText` que deve ser definida no construtor do seu comando. Caso você não atribua nenhum valor para essa propriedade, o padrão será exibir o nome do comando.
* **C:** Será exibido todas os argumentos (propriedades) do comando, um em baixo do outro.
  * **C1:** A fonte desse texto vem do atributo `ArgumentAtrribute(LongName="")`.
  * **C2:** A fonte desse texto vem do atributo `ArgumentAtrribute(ShortName="")`.
  * **C3:** A fonte desse texto vem do atributo `ArgumentAtrribute(Help="")`.
  * **C4:** Esse texto só vai aparecer se a flag `ArgumentAtrribute(ShowHelpComplement=true)` estiver ligada. O texto que será exibido vai depender da configuração do membro:
    * `Strings.HelpArgDescRequired`: Quando o membro é obrigatório
    * `Strings.HelpArgDescOptionalWithDefaultValue`: Quando o membro é opcional e tem default value.
    * `Strings.HelpArgDescOptionalWithoutDefaultValue`: Quando o membro é opcional e não tem default value.
* **D:** A fonte desse texto vem do atributo `ActionAtrribute(Name="")`.
* **E:** São as mesmas fontes dos argumentos de comando (propriedades), pois ambos os membros utilizam o mesmo atributo.
* **F:** Texto complementar para explicar como o help funciona. A fonte desse texto vem da classe `Strings.HelpFooterDesc`.

**Exemplo:**

```csharp
public class HelpCommand : Command
{
    // With complement
    [Argument(Help = "My property1 help")]
    public string MyProperty1 { get; set; }

    // Without complement
    [Argument(Help = "My property2 help", ShowHelpComplement = false, IsRequired = true)]
    public int MyProperty2 { get; set; }

    public HelpCommand()
    {
        this.HelpText = "Help for this command";
    }

    [Action(Help = "Action help")]
    public void MyActionHelp
    (
        [Argument(Help = "Argument help")]
        string arg0, // With complement

        [Argument(Help = "Argument help", ShowHelpComplement = false)]
        string arg1  // Without complement
    )
    {

    }
}
```

```
usage:    --my-property2=<number> [--my-property1=<phrase>] [-v,
          --verbose=<None|All|Info|Success|Critical|Warning|Error|Quiet>]
          <actions[args]>

Help for this command

   --my-property1    My property1 help. Is optional.
   --my-property2    My property2 help

   my-action-help    Action help
      --arg0         Argument help. Is required.
      --arg1         Argument help
```


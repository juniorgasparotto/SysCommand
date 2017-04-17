## Customizando as informações de help de actions e seus parametros <header-set anchor-name="methods-customizing-help" />

Para configurar o texto de help utilize o atributo `ActionAttribute(Help="my help")`. Caso você não informe esse atributo, sua ação ainda será exibido no help, mas sem informações de ajuda.

Para cada paramentro utilizasse o mesmo atributo das propriedades `ArgumentAttribute(Help="")`. O comportamento é exatamente o mesmo. Veja <anchor-get name="properties-customizing-help" />.

**Exemplo:**

```csharp
public class MethodHelpCommand : Command
{
    public MethodHelpCommand() 
    {
        this.HelpText = "Help for this command";
    }

    [Action(Help = "Action help")]
    public string MyActionHelp(
        [Argument(Help = "Argument help")]
        string arg0, // With complement

        [Argument(Help = "Argument help", ShowHelpComplement = false)]
        string arg1  // Without complement
    )
    {
        return "Action help";
    }
}
```

```
C:\MyApp.exe help
Help for this command

   my-action-help                  Action help
      --arg0                       Argument help. Is required.
      --arg1                       Argument help
```

Para mais informações sobre o help veja no tópico <anchor-get name="help" />.
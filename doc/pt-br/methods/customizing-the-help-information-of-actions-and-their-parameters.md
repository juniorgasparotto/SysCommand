######Customizando as informações de help de actions e seus parametros

Para as `actions` você precisa customizar o atributo `ActionAttribute(Help="")` e para os paramentros utilizasse o atributo `ArgumentAttribute(Help="")`. Por padrão, para cada argumento será exibido um complemento após o texto do help. A informação que esse complemento nos tras é se o parametro é obrigatório ou opcional (com ou sem default value). Caso você deseje desativar esse complemento utilize o atributo `ArgumentAttribute(ShowHelpComplement=false)`.

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

Esse tópico apenas apresentará os atributos que configuram o help. Para mais informações sobre o help veja no tópico `Help Automatico`.
## Customizando as informações de help <header-set anchor-name="properties-customizing-help" />

Para configurar o texto de help utilize o atributo `ArgumentAttribute(Help="my help")`. Caso você não informe esse atributo, seu argumento ainda será exibido no help, mas sem informações de ajuda. 

Contudo, ainda será exibido um texto de complemento para cada argumento, esse texto informa se o argumento é obrigatório ou opcional (com ou sem default value). Esse texto é exibido por padrão, mas você pode desativa-lo com o atributo `ArgumentAttribute(ShowHelpComplement=false)`.

```csharp
public class CustomPropertiesHelpCommand : Command
{
    // customized with long and short option
    [Argument(Help = "This is my property")]
    public int? MyPropertyHelp { get; set; }

    [Argument(Help = "This is my property 2", ShowHelpComplement = false)]
    public int? MyPropertyHelp2 { get; set; }

    public CustomPropertiesHelpCommand()
    {
        this.HelpText = "Custom help for CustomPropertiesHelpCommand";
    }
}
```

```
C:\MyApp.exe help
Custom help for CustomPropertiesHelpCommand

   --my-property-help              This is my property. Is optional.
   --my-property-help2             This is my property 2
```

Para mais informações sobre o help veja no tópico <anchor-get name="help" />.
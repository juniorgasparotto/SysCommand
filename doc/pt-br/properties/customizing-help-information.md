## Customizando as informações de help <header-set anchor-name="properties-customizing-help" />

Para configurar o texto de help utilize o atributo `ArgumentAttribute(Help="")`. Por padrão, para cada argumento será exibido um complemento após o texto do help. A informação que esse complemento nos tras é se o parametro é obrigatório ou opcional (com ou sem default value). Caso você deseje desativar esse complemento utilize o atributo `ArgumentAttribute(ShowHelpComplement=false)`.

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

Esse tópico apenas apresentará os atributos que configuram o help. Para mais informações sobre o help veja no tópico `Help Automatico`.
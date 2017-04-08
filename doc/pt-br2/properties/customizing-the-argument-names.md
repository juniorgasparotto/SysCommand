## Customizando os nomes dos argumentos <header-set anchor-name="properties-customizing-name" />

A regra a seguir descreve como é o comportamento padrão de nomenclatura para que uma propriedade vire um `argument`:

Primeiro se converte o nome da propriedade em minusculo, depois adiciona um traço "-" antes de cada letra maiuscula que estiver no meio ou no final do nome. No caso de propriedades com apenas uma letra, o padrão será deixar a letra minuscula e o input será aceito apenas na forma curta.

Essa é a regra padrão de nomenclarutura e você pode escolher usa-la ou customizada-la, para isso utilize o atributo `ArgumentAttribute`. O uso do atributo `ArgumentAttribute` é exclusivo, ao utiliza-lo você esta eliminando o padrão de nomenclatura por completo, ou seja, se você customizar a `forma curta` você será obrigado a customizar a `forma longa` também, e vice-versa. Do contrário só o formato customizado será habilitado.

**Exemplo:**

```csharp
public class CustomPropertiesNamesCommand : Command
{
    // customized with long and short option
    [Argument(LongName = "prop", ShortName = 'A')]
    public int? MyCustomPropertyName { get; set; }

    // only with long option
    public string NormalLong { get; set; }

    // customized only with short option
    [Argument(ShortName = 'B')]
    public string ForcedShort { get; set; }

    // only with short option
    public int? C { get; set; }

    public CustomPropertiesNamesCommand()
    {
    }

    public void Main()
    {
        if (MyCustomPropertyName != null)
            App.Console.Write("MyCustomPropertyName=" + MyCustomPropertyName);

        if (NormalLong != null)
            App.Console.Write("NormalLong=" + NormalLong);

        if (ForcedShort != null)
            App.Console.Write("ForcedShort=" + ForcedShort);

        if (C != null)
            App.Console.Write("C=" + C);
    }
}
```

```
C:\MyApp.exe --prop 111 --normal-long strvalue -B strvalue2 -c 9999
MyCustomPropertyName=111
NormalLong=strvalue
ForcedShort=strvalue2
C=9999

C:\MyApp.exe -A 111 --normal-long strvalue -B strvalue2 -c 9999
MyCustomPropertyName=111
NormalLong=strvalue
ForcedShort=strvalue2
C=9999
```
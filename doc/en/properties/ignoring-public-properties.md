## Ignorar propriedades publicas por uma escolha manual usando atributo !heading

Para mudar o comportamente padrão de propriedades publicas, você precisa apenas desligar a flag `OnlyPropertiesWithAttribute` do `Command`. Com ela desligada o parseador deixará de olhar para as propriedades publicas e usará apenas as propriedades publicas e que tiverem o atributo `ArgumentAtrribute`.

**Exemplo:**

```csharp
public class TestProperty4Command : Command
{
    public int? MyPropertyWithoutAttribute { get; set; }

    [Argument]
    public int? MyPropertyWithAttribute { get; set; }

    public TestProperty4Command()
    {
        this.OnlyPropertiesWithAttribute = true;
    }

    public void Main()
    {
        if (MyPropertyWithAttribute != null)
            App.Console.Write("MyPropertyWithAttribute=" + MyPropertyWithAttribute);
    }
}
```

```
C:\MyApp.exe --my-property-with-attribute 1
MyPropertyWithAttribute=1

C:\MyApp.exe --my-property-without-attribute 1
There are errors in command: DoSomethingCommand
The argument '--my-property-without-attribute' does not exist
```
######Propriedades obrigatórias

Para argumentos que são obrigatórios, é necessário que você use o `ArgumentAtrribute` ligando a flag `IsRequired`.

**Exemplo:**

```csharp
public class TestProperty5Command : Command
{
    [Argument(IsRequired = true)]
    public string MyPropertyRequired { get; set; }

    public void Main()
    {
        if (MyPropertyRequired != null)
            App.Console.Write("MyPropertyRequired=" + MyPropertyRequired);
    }
}
```

```
C:\MyApp.exe
There are errors in command: TestProperty5Command
The argument '--my-property-required' is required

C:\MyApp.exe --my-property-required 123
MyPropertyRequired=123
```
## Usando inputs posicionais <header-set anchor-name="methods-positional-inputs" />

Outro modo de chamar sua action no console é usando `input posicional`. Por padrão, todas as `action` aceitam argumentos posicionais, mais isso pode ser desabilitado usando o atributo `ActionAttribute(EnablePositionalArgs = false)`.

**Exemplo:**

```csharp
public string MyActionWithPosicional(int arg0, int arg1)
{
    return "MyActionWithPosicional";
}

[Action(EnablePositionalArgs = false)]
public string MyActionWithoutPosicional(int arg0, int arg1)
{
    return "MyActionWithoutPosicional";
}
```

```
C:\MyApp.exe my-action-with-posicional --arg0 1 --arg1 2
MyActionWithPosicional

C:\MyApp.exe my-action-with-posicional 1 2
MyActionWithPosicional

C:\MyApp.exe my-action-without-posicional --arg0 1 --arg1 2
MyActionWithoutPosicional

C:\MyApp.exe my-action-without-posicional 1 2
There are errors in command: Method1Command
Error in method: my-action-without-posicional(Int32, Int32)
The argument '--arg0' is required
The argument '--arg1' is required
```
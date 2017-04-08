## Métodos padrão <header-set anchor-name="methods-default" />

O uso de métodos padrão (ou métodos implicitos) fazem com que o recurso seja muito similar ao uso de propriedades, ou seja, você não é obrigado a especificar o nome da `action` e os seus parâmetros podem ser inseridos diretamente no input como se fossem argumentos provenientes de propriedades.

Por convensão, se você chamar sua `action` de "Main" e ela tiver parametros, ela será considerada como padrão. Para mudar esse comportamento você deve desligar a flag `Action(IsDefault = false)`, assim o comportamento padrão será alterado e sua action "Main" (com parametros) não será mais acessada de forma implicita e obrigará a especificação de seu nome no input. O contrário também é verdadeiro, se sua action tem outro nome e você gostaria de torna-la um método padrão então basta ligar a flag `Action(IsDefault = true)`.

**Exemplo:**

```csharp
public class MethodDefaultCommand : Command
{
    public string Main(string arg0)
    {
        return "Main(string arg0)";
    }

    public string Main(string arg0, string arg1)
    {
        return "Main(string arg0, string arg1)";
    }

    [Action(IsDefault = false)]
    public string Main(int argument)
    {
        return "Main(int argument)";
    }

    [Action(IsDefault = true)]
    public string AnyName(string argument)
    {
        return "AnyName(string argument)";
    }

    [Action(IsDefault = true)]
    public string ActionWhenNotExistsInput()
    {
        return "ActionWhenNotExistsInput()";
    }
}
```

```
C:\MyApp.exe --arg0 value
Main(string arg0)

C:\MyApp.exe --arg0 value --arg1 value1
Main(string arg0, string arg1)

C:\MyApp.exe main --argument 9999
Main(int argument)

C:\MyApp.exe --argument value
AnyName(string argument)

C:\MyApp.exe
ActionWhenNotExistsInput()
```

**Observações:**

* É importante ressaltar que o todos os métodos padrão ainda podem ser chamados de forma explicita, ou seja, com o seu nome sendo especifico.
* O uso de método padrão sem argumentos só funciona se não existir nenhum argumento required, do contrário esse método nunca será chamado, pois haverá um erro obrigando o uso do argumento.'
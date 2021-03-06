### Multi-action <header-set anchor-name="using-the-multi-action-feature" />

O recurso de multi-action permite que você consiga disparar mais de uma `action` em um mesmo input. Por padrão, ele vem habilitado e caso você ache desnecessário então é só desliga-lo. É importante ressaltar que o recurso <anchor-get name="argument-history-manager" /> deixará de funcionar caso isso ocorra.

Outro ponto importante é a necessidade de "escapar" seu input caso o valor que você deseje inserir conflite com um nome de uma `action`. Essa regra vale para valores de argumentos de qualquer natureza (propriedades ou parâmetros).

**Exemplo:**

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        new App().Run(args);

        // EnableMultiAction = false
        /*
        new App(null, false).Run(args);
        */
    }

    public class MyCommand : Command
    {
        public string Action1(string value = "default")
        {
            return $"Action1 (value = {value})";
        }

        public string Action2(string value = "default")
        {
            return $"Action2 (value = {value})";
        }
    }
}
```

```
MyApp.exe action1
Action1 (value = default)

MyApp.exe action2
Action2 (value = default)

MyApp.exe action1 action2
Action1 (value = default)
Action2 (value = default)

MyApp.exe action1 action2 action1 action1 action2
Action1 (value = default)
Action2 (value = default)
Action1 (value = default)
Action1 (value = default)
Action2 (value = default)

MyApp.exe action1 --value \\action2
Action1 (value = action2)
```

O último exemplo demostra como usar o scape em seus valores que conflitam com nomes de ações. Um fato importante é que no exemplo foi usado duas barras invertidas para fazer o scape, mas isso pode variar de console para console, no `bash` o uso de apenas uma barra invertida não tem nenhum efeito, provavelmente ele deve usar para outros scapes antes de chegar na aplicação.
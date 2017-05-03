## Inicializando com o simulador de console <header-set anchor-name="initializing-by-static-method" />

Este recurso ajuda você a testar seus inputs dentro do próprio Visual Studio sem a necessidade de executar seu ".exe" em um console externo. É importante ressaltar que esse simulador só será exibido dentro do Visual Studio.

A chamada é bastante simples, basta adicionar uma linha para que tudo funcione usando as regras padrão. Caso você queira customizar a sua instância de `App` então utilize o construtor `App.RunApplication(Func<App> appFactory)`.

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        // Default
        App.RunApplication();

        // Or use custom App

        /*
        App.RunApplication(() =>
        {
            var app = new App(enableMultiAction: false);
            return app;
        });
        */
    }

    public class MyCommand : Command
    {
        public string MyProperty
        {
            set
            {
                App.Console.Write(value);
            }
        }
    }
}
```

Quando você executar esse código no Visual Studio, um prompt com o label `cmd>` será exibido. Isso indica que você pode iniciar seus testes quantas vezes for necessário. Para sair, você pode usar o atalho padrão "CTRL+C" ou apertar o botão "stop" do Visual Studio.

```
cmd> --my-property value
value
cmd> --my-property otherValue
otherValue
```
## Inicializando com simulador de console <header-set anchor-name="initializing-by-static-method" />

O método estático `App.RunApplication` disponibiliza um recurso muito interressante de `simulação de console` que ajuda você a testar seus inputs dentro do próprio Visual Studio sem a necessidade de executar seu ".exe" em um console externo ou via "Command Line Arguments". É importante ressaltar que esse simulador só será exibido dentro do Visual Studio, quando executar seu aplicativo final em outro console o comportamento será o mesmo do modo por instancia.

A chamada é bastante simples, basta uma linha para que tudo funcione usando as regras padrão. Caso você queira customizar a sua instancia de `App` então utilize o construtor `App.RunApplication(Func<App> appFactory)`.

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

Ao iniciar esse código no Visual Studio um prompt padrão com um label `cmd>` será exibido. Isso indica que você pode iniciar seus testes quantas vezes for necessário. Para sair você pode usar o atalho padrão "CTRL+C" ou apertar o botão "stop" do Visual Studio.

```
cmd> --my-property value
value
cmd> --my-property otherValue
otherValue
```
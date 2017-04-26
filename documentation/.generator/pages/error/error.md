# Tratamento de erros <header-set anchor-name="error" />

O tratamento de erro é gerado de forma automática pelo sistema e são categorizados da seguinte forma:

* Erros no processo de parse: São erros que ocorrem no processo de parse e são sub-categorizados da seguinte forma:
  * `ArgumentParsedState.ArgumentAlreadyBeenSet`: Indica que um argumento esta duplicado no mesmo input.
  * `ArgumentParsedState.ArgumentNotExistsByName`: Indica que um argumento nomeado não existe.
  * `ArgumentParsedState.ArgumentNotExistsByValue`: Indica que um argumento posicional não existe
  * `ArgumentParsedState.ArgumentIsRequired`: Indica que um argumento é obrigatório
  * `ArgumentParsedState.ArgumentHasInvalidInput`: Indica que um argumento esta inválido
  * `ArgumentParsedState.ArgumentHasUnsupportedType`: Indica que o esta tudo certo com o input, porém o tipo do argumento não tem suporte. Veja a lista de tipos suportados em <anchor-get name="support-types" />.
* Not Found: Nenhuma rota encontrada para o input solicitado.
* Exception génerica: Não existe nenhum tipo de tratamento padrão, mas é possível interceptar qualquer exception dentro do evento `App.OnException`.

O responsável por formatar e imprimir os erros é o handler padrão `SysCommand.ConsoleApp.Handlers.DefaultApplicationHandler` que intercepta o resultado final da execução e caso tenha erros chama o método `ShowErrors(ApplicationResult appResult)` ou `ShowNotFound(ApplicationResult appResult)` da classe `SysCommand.ConsoleApp.Descriptor.DefaultDescriptor`.

Caso queira customizar as mensagens de erro, você pode trocar o handler `DefaultApplicationHandler` por completo (não recomendado) ou criar uma classe que herde de `DefaultDescriptor` subrescrevendo apenas os métodos de erros.

**Exemplo:**

```csharp
using SysCommand.ConsoleApp;
public class Program
{
    public static int Main()
    {
        return App.RunApplication(
            () => {
                var app = new App();
                app.Descriptor = new CustomDescriptor();
                app.OnException += (appResult, exception) =>
                {
                    app.Console.ExitCode = ExitCodeConstants.Error;
                    app.Console.Write(exception.Message);
                };
                return app;
            }
        );
    }

    public class CustomDescriptor : DefaultDescriptor
    {
        public override void ShowErrors(ApplicationResult appResult)
        {
            foreach (ExecutionError error in appResult.ExecutionResult.Errors)
            {
                foreach (ArgumentParsed prop in error.PropertiesInvalid)
                {
                    if (prop.ParsingStates.HasFlag(ArgumentParsedState.ArgumentAlreadyBeenSet))
                        appResult.App.Console.Error(string.Format("The argument '{0}' has already been set", prop.GetArgumentNameInputted()));
                    if (prop.ParsingStates.HasFlag(ArgumentParsedState.ArgumentNotExistsByName))
                        appResult.App.Console.Error(string.Format("The argument '{0}' does not exist", prop.GetArgumentNameInputted()));
                    if (prop.ParsingStates.HasFlag(ArgumentParsedState.ArgumentNotExistsByValue))
                        appResult.App.Console.Error(string.Format("Could not find an argument to the specified value: {0}", prop.Raw));
                    if (prop.ParsingStates.HasFlag(ArgumentParsedState.ArgumentIsRequired))
                        appResult.App.Console.Error(string.Format("The argument '{0}' is required", prop.GetArgumentNameInputted()));
                    if (prop.ParsingStates.HasFlag(ArgumentParsedState.ArgumentHasInvalidInput))
                        appResult.App.Console.Error(string.Format("The argument '{0}' is invalid", prop.GetArgumentNameInputted()));
                    if (prop.ParsingStates.HasFlag(ArgumentParsedState.ArgumentHasUnsupportedType))
                        appResult.App.Console.Error(string.Format("The argument '{0}' is unsupported", prop.GetArgumentNameInputted()));
                }

                foreach (ActionParsed method in error.MethodsInvalid)
                {
                    foreach (ArgumentParsed parameter in method.Arguments)
                    {
                        ...
                    }
                }
            }
        }

        public override void ShowNotFound(ApplicationResult appResult)
        {
            appResult.App.Console.Error("Could not find any action.", forceWrite: true);
        }
    }
}
```
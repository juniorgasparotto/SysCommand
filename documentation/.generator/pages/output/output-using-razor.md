## Usando template Razor <header-set anchor-name="output-razor" />

Outra opção para exibir outputs é a utilização de templates `Razor`. Esse mecanismo foi projetado para coisas simples, é muito importante dizer que ele não dispõe de diversos recursos como: debug, intellisense, highlight e analise de erros.

Para utilizar `Razor` deve-se seguir alguns simples passos:

* Por organização, criar uma pasta chamada "Views".
* Caso ainda queira mais organização, crie uma sub-pasta dentro da pasta "Views" com o nome do `Command`.
* Criar um arquivo de template com a extensão ".razor" dentro da pasta "Views". Esse arquivo deve ter o mesmo nome da action (método)
* Implementar o seu template podendo ou não usar a variável "@Model"
* Exibir as propriedades do arquivo ".razor" e configura-lo com a **Build Action = Embedded Resource** ou com a propriedade **Copy to Output = Copy aways**. Isso é necessário para o gerenciador de template encontre o arquivo na basta "bin/" em caso do uso do **Copy to Output** ou dentro do Assembly do domínio de aplicativo padrão com o uso do **Build Action**.

**Exemplo:**

######Commands/ExampleRazorCommand.cs

```csharp
public class ExampleRazorCommand : Command
{
    public string MyAction()
    {
        return View<MyModel>();
    }

    public string MyAction2()
    {
        var model = new MyModel
        {
            Name = "MyName"
        };

        return View(model, "MyAction.razor");
    }

    public class MyModel
    {
        public string Name { get; set; }
    }
}
```

######Views/ExampleRazor/MyAction.razor

```
@if (Model == null)
{
    <text>#### HelloWorld {NONE} ####</text>
}
else {
    <text>#### HelloWorld (@Model.Name) ####</text>
}
```

######Tests

Input1:

```
MyApp.exe my-action
```

Input2:

```
MyApp.exe my-action2
```

Outputs:

```
    #### HelloWorld {NONE} ####
    #### HelloWorld {MyName} ####
```

######Observação

* A pesquisa do template via `Arquivo físico` ou via `Embedded Resource` segue a mesma lógica. Ele busca pelo caminho mais especifico usando o nome do "command.action.extensão" e caso ele não encontre ele tentará encontrar pelo nome mais generico, sem o nome do command.
  * Busca primeiro por: "ExampleRazorCommand.MyAction.razor"
  * Caso não encontre na primeira tentativa, então busca por: "MyAction.razor"
* É possível passar o nome da view diretamente, sem a necessidade de usar a pesquisa automática. como no exemplo da action "MyAction2()".
* Por questões técnicas, o método View<>() obriga o uso de uma inferencia ou um model. Infira um `object` se você não necessitar de um model `View<object>()`.
* Devido ao uso do recurso de `Razor`, o seu projeto terá uma dependencia da dll `System.Web.Razor`.
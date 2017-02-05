# Output usando template T4 <header-set anchor-name="output-t4" />

Outra opção para exibir outputs é a utilização de templates `T4`. Esse mecanismo, ao contrário dos templates `Razor` é mais completo, ele não perdeu nenhum dos beneficios que o Visual Studio nos fornece. Basta seguir apenas alguns passos para usa-lo:

* Por organização, criar uma pasta "Views"
* Criar um arquivo T4 no formato "Runtime Text Template"
* Se for utilizar model é preciso configurar um parametro, que por obrigatoriedade, deve-se chamar "Model" e ter o seu respectivo tipo em forma de string. Caso não utilize nenhum "Model" então ignore esse passo.
* Implementar o seu template

**Exemplo:**

######Commands/ExampleT4Command.cs

```csharp
public class ExampleT4Command : Command
{
    public string T4MyAction()
    {
        return ViewT4<MyActionView>();
    }

    public string T4MyAction2()
    {
        var model = new MyModel
        {
            Name = "MyName"
        };

        return ViewT4<MyActionView, MyModel>(model);
    }

    public class MyModel
    {
        public string Name { get; set; }
    }
}
```
######Views/ExampleT4/MyActionView.tt

```csharp
<#@ parameter type="Example.T4Command.MyModel" name="Model" #>
<# if(Model == null) { #>
#### HelloWorld {NONE} ####
<# } #>
<# else { #>
#### HelloWorld (<#= Model.Name #>) ####
<# } #>
```

######Tests

Input1: ```MyApp.exe t4-my-action```

Input2: ```MyApp.exe t4-my-action2```

Outputs:

```
    #### HelloWorld {NONE} ####
    #### HelloWorld {MyName} ####
```
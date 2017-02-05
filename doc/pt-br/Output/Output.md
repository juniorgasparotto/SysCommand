# Output <header-set anchor-name="output" />

O mecanismo de output foi extendido para aumentar a produtividade.

Primeiro, foi criado um pequeno wrapper da classe `System.Console` chamado `SysCommand.ConsoleApp.ConsoleWrapper` que esta disponivel dentro do contexto da aplicação na propriedade `App.Console`. Esse wrapper pode ser herdado e ter seus recursos modificados ou potencializados, mas por padrão temos as seguintes funcionalidades:

* Métodos de write para cada tipo de verbo
* Possibilidade de customização da cor do texto de cada verbo
  * App.Console.ColorInfo
  * App.Console.ColorCritical
  * App.Console.ColorError
  * App.Console.ColorSuccess
  * App.Console.ColorWarning
  * App.Console.ColorRead
* Variavel de controle de tipo de saída `App.Console.ExitCode` onde você pode usa-la como retorno do seu método `int Main(string[] args)`:
  * "0" : Sucesso
  * "1" : Erro
* Quebra de linha inteligente durante o uso dós métodos de write e read. A variável `App.Console.BreakLineInNextWrite` controla as quebras e te ajuda a não deixar linhas vazias sem necessidade.

Outro recurso seria a utilização dos `returns` das actions e que serão, caso existam, utilizados como output. Esse recurso se assemelha muito ao "AspNet MVC".

**Exemplo:**

```csharp
public class TestOutput : Command
{
    public decimal Test()
    {
       var result = this.App.Console.Read("My question: ");

        if (result != "S")
        { 
            // option1: use write method in wrapper class
            this.App.Console.Write(1.1m);

            // option2: use .NET class directly
            Console.WriteLine(2.2m);
        }

        // option3: or use return, its the same the option1
        return 3.3m;
    }
}
```

Input: ```MyApp.exe test```

Outputs:

```
My question: N
1.1
2.2
3.3
```

Por último, vale lembrar que nada disso impede você de usar os mecanismos comuns do .NET, como a classe "System.Console".

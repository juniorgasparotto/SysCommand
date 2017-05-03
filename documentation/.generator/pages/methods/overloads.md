## Sobrecargas <header-set anchor-name="methods-overloads" />

O recurso de sobrecarga de métodos é suportado da mesma forma que você faria para qualquer outra finalidade. Muitas vezes esse recurso pode ser mais interessante que usar parâmetros opcionais, o código fica mais limpo. Outras vezes isso não será possível, pois com parâmetros opcionais o usuário tem a opção de escolher qualquer parâmetro independentemente de sua posição no método, coisa que a sobrecarga não pode.

**Exemplo:**

```csharp
public class Method1Command : Command
{
    public string MyAction3()
    {
        return "MyAction3";
    }

    public string MyAction3(int arg0)
    {
        return "arg0 has input";
    }

    public void MyAction3(int arg0, int arg1)
    {
        App.Console.Write("arg0 has input");
        App.Console.Write("arg1 has input");
    }
}
```

```
C:\MyApp.exe my-action3
MyAction3

C:\MyApp.exe my-action3 --arg0 9
arg0 has input

C:\MyApp.exe my-action3 --arg0 9 --arg1 99
arg0 has input
arg1 has input

C:\MyApp.exe my-action3 --arg1 99
There are errors in command: Method1Command
The argument '--arg1' does not exist
```

O último comando mostrou a limitação da sobrecarga com relação aos parâmetros opcionais. O analisador entendeu que os dois métodos com parâmetros `MyAction3` estão inválidos, veja:

* MyAction3(int arg0): Não tem o input "--arg1" que foi solicitado, portanto esta inválido.
* MyAction3(int arg0, int arg1): Tem o input "--arg1", mas não tem o input "--arg0", portanto esta inválido.

Nesse caso o analisador escolhera o unico método valido, ou seja, o método `MyAction3` _sem parâmetros_ e usará o argumento extra "--arg1" para tentar encontra-lo como propriedade em algum `Command`, porém essa propriedade não existe em nenhum lugar, gerando o erro.
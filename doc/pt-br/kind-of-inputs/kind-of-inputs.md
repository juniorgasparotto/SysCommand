# Tipos de inputs !heading

Os argumentos, sejam eles paramentros de métodos ou propriedades, podem ter duas formas: a `longa` e a `curta`. Na forma `longa` o argumento deve-se iniciar com "--" ou "/" seguido do seu nome. Na forma `curta` ele deve sempre iniciar com apenas um traço "-" e seguido de apenas um caracter. Esse tipo de input (longo ou curto) é chamado de `input nomeado`.

Existe também a possibilidade de aceitar inputs posicionais, ou seja, sem a necessidade de utilizar os nomes dos argumentos. Esse tipo de input é chamado de `input posicional`.

**Exemplo:**

```csharp
public string MyProperty { get;set; }
public void MyAction(string A, string B);
```

**Input nomeado**:

```MyApp.exe my-action -a valueA -b valueB --my-property valueMyProperty```

**Input posicional**:

```MyApp.exe my-action valueA valueB valueMyProperty```

* Para as propriedades, o `input posicional` é desabilitado por padrão, para habilita-lo utilize a propriedade de comando `Command.EnablePositionalArgs`. 
* Para os métodos esse tipo de input é habilitado por padrão, para desabilita-lo veja no tópico de `Customizações`. 
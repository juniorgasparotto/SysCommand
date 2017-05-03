## `Arguments` <header-set anchor-name="input-arguments" />

Os argumentos representam o meio mais básico de uma aplicação console, são normalmente representados da seguinte forma:

```
C:\MyApp.exe --argument-name value     // Long
C:\MyApp.exe -v value                  // Short
C:\MyApp.exe value                     // Positional
```

Programaticamente, os `arguments` podem ser derivados de propriedades ou dos parâmetros dos métodos.

### Argumento nomeado <header-set anchor-name="input-named" />

Argumentos nomeados são caracterizado por duas formas: a **longa** e a **curta**. Na forma **longa** o argumento deve-se iniciar com `--` seguido do seu nome. Na forma **curta** ele deve iniciar com apenas um traço `-` ou uma barra `/` seguido de apenas um caracter que representa o argumento.

Os valores dos argumentos devem estar na frente do nome do argumento separados por um espaço ` ` ou pelos caracteres `:` ou `=`.

**Exemplo:**

```csharp
public string MyProperty { get;set; }
public string v { get;set; }
```

_Input longo:_

```
MyApp.exe --my-property value
MyApp.exe -v value
```

_Input curto:_

```
MyApp.exe -v value
```

_OU usando o delimitador `/` e os separadores `=` e `:`_

```
MyApp.exe --my-property=value
MyApp.exe /v:value
```

### Argumento posicional <header-set anchor-name="input-positional" />

Argumentos posicionais funcionam sem a necessidade de utilizar os nomes dos argumentos. Basta inserir seus valores diretamente. Só é preciso tomar cuidado com esse recurso, pois pode confundir o usuário em caso de muitos argumentos posicionais.

**Exemplo:**

```csharp
public string PropA { get;set; }
public string PropB { get;set; }
public string PropC { get;set; }
```

_Input nomeado:_

```
MyApp.exe --prop-a ValueA --prop-b ValueB --prop-c ValueC
```

_Input posicional:_

```
MyApp.exe ValueA ValueB ValueC
```

_Observações:_

* Para as propriedades, o **input posicional** é desabilitado por padrão, para habilitar esse recurso, utilize a configuração `Command.EnablePositionalArgs`.
* Para os métodos, esse tipo de input é habilitado por padrão, para desabilita-lo veja no tópico de <anchor-get name="methods-positional-inputs" />.
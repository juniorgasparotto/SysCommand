# Input <header-set anchor-name="input" />

Chamamos de input todas as linhas de comandos que o usuário digita e envia para o aplicativo. Os formatos de input se dividem entre `arguments` e `actions`.

Os `arguments` representam o meio mais básico de uma aplicação console, são normalmente representados da seguinte forma:

```
C:\MyApp.exe --argument-name value     // Long
C:\MyApp.exe -v value                  // Short
C:\MyApp.exe value                     // Positional
```
Programaticamente, os `arguments` podem ser derivados de `properties` ou dos parâmetros dos `methods`.

Já as `actions` são palavras reservadas para executar uma determinada ação em seu aplicativo. Elas não precisam de nenhum sufixo como ocorre com os `arguments`,basta usa-las diretamente em seu input. Um bom exemplo de `action` são os recursos do `git` como:

```
git add -A; 
git commit -m "comments"
```

Onde `add` e `commit` seriam o nome das `actions` e `-A` e `-m` seus respectivos `arguments`.

Programaticamente, as `actions` são derivadas dos `methods`.

### Input nomeado <header-set anchor-name="input-named" />

O input nomeado é caracterizado por duas formas: a `longa` e a `curta`. Na forma `longa` o argumento deve-se iniciar com `--` seguido do seu nome. Na forma `curta` ele deve iniciar com apenas um traço `-` ou uma barra `/` seguido de apenas um caracter que representa o argumento.

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

### Input posicional <header-set anchor-name="input-positional" />

Os inputs posicionais funcionam sem a necessidade de utilizar os nomes dos argumentos. Basta inserir seus valores diretamente. Só é preciso tomar cuidado com esse recurso, pois pode confundir o usuário em caso de muitos argumentos posicionais.

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

* Para as propriedades, o `input posicional` é desabilitado por padrão, para habilita-lo utilize a propriedade de comando `Command.EnablePositionalArgs`.
* Para os métodos esse tipo de input é habilitado por padrão, para desabilita-lo veja no tópico de <anchor-get name="methods-positional-inputs" />.
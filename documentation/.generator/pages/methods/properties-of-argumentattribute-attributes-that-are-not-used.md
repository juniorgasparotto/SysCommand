## Propriedades do atributos ArgumentAttribute que não são utilizados <header-set anchor-name="methods-not-used-attrs" />

As seguintes propriedades não fazem sentido no cenário de parâmetros de métodos e só existem por que o atributo `ArgumentAtrribute` é compartilhado no uso de propriedades.

* IsRequired: Em C#, todo parâmetro que não tem **valor padrão** é obrigatório, essa configuração é ignorada se for utilizada.
* DefaultValue: Como o proprio C# já nos dá a opção de **valor padrão** em parâmetros, essa configuração é redundante, sendo assim ela é ignorada por que o padrão do .NET já é suficiente e mais limpo.
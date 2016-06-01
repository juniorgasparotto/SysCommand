Funcionalidade básica:

Ao criar-se uma classe de comando de ação (que herda de CommandAction) todos os seus métodos publicos (e seus paraâmetros) serão habilitados para serem chamados via prompt de comando. Contudo, é possível customizar esse processo da seguinte forma:

- O método, mesmo sendo publico, pode ser ignorado  usando o atributo "Action(Ignore=true)".
- O padrão usado no prompt de comando será o nome do método em modo de lowercase, porém as letras maiusculas contidas no meio do método terão um "-" as antecedendo.
- O padrão pode ser sobrescrito pelo atributo 'Action(Name="...")'
- O nome do método pode ser alterado caso o "command" use prefixo em seu construtor (AddPrefixInActions). Isso afeta todas as ações, ou seja, será adicionado um prefixo antes do nome da ação. 
	Esse prefixo será o nome da classe convertida em lowercase e removendo o sufixo "Command" caso ele exista no final do nome da classe.
	Esse prefixo pode ser alterado caso a propriedade "PrefixActions" seja especificada.
	Esse prefixo será ignorado quando o atributo "Action(AddPrefix = false)" for especificado
	
- Todos os argumentos dos métodos seguirão o formato especificado pelo framework "FluentCommandLineParser": '-', '/', ':', '='.
	"-": Para argumentos de apenas um caracter (shortName)
	"--": Para argumentos de mais de um caracter (longName)
	"=", ":", "/": Para argumentos de mais de um caracter (longName)
- Argumentos com apenas um caracter serão convertidos automáticamente para "shortName", ou seja, será acessado apenas por um traço "-".
- Argumentos podem ser customizados usando o atributo "Argument(ShortName="...", LongName="...",Help="...", ShowHelpComplement=true|false)"
	- ShortName: Cria/Altera uma entrada no prompt para argumentos de apenas um caractere
	- LongName: Cria/Altera uma entrada no prompt para argumentos de mais de um caractere
	- Help: Cria um help para o argumento
	- ShowHelpComplement: Exibe (true) ou não (false) o label "Is required" para argumentos sem default value ou "Is optional (Default: '...'" para argumentos com valor default. O default é true.
	- Existem outras propriedades nesse atributo, porém elas não são suportadas para argumentos de métodos/ações.
- Métodos publicos com o nome "Default" ou com o atributo "Action(IsDefault=true)" podem ser acessados sem a especificação do nome da ação, ou seja, passando direto os argumentos.

- Em uma requisição, apenas um método/ação pode ser escolhido para cada command.
- Em caso de sobrecarga de método/ação no mesmo "command" ganhará o método com a maior proximidade de argumentos com base no que foi enviado. 
- Em caso de haver dois ou mais "commands" com o mesmo nome de ação, ambos serão escolhidos e chamados.

- Em caso de haver dois ou mais métodos default no mesmo "command" ganhará o método que tem menos argumentos.

class HelloWordCommand : CommandAction
{
	public HelloWordCommand() {
		this.AddPrefixInActions = true;
		// this.PrefixActions = "custom-prefix";
	}
	
	public void Default(string p1) {
		...
	}
	
	[Action(IsDefault=true)]
	public void OtherDefault(string p1) {
		...
	}
	
	[Action(Ignore=true)]
	public void IsIgnored(string p1, string p2, int p) {
		...
	}
	
	public void FirstMethod(string p1, string p2, int p) {
		...
	}
	
	[Action(Name="action-with-other-name", AddPrefix = false)]
	public void FirstMethod2
	(
		[Argument(ShortName="p")] string p1,
								  string p2, 
		[Argument(LongName="parameter")] int p
	)
	{
		...
	}
}

$~: MyApp.exe default [arguments...]
$~: MyApp.exe [arguments...]
$~: MyApp.exe other-default [arguments...]
#~: MyApp.exe hello-word-first-method --p1 "..." --p2 "..." -p 0
#~: MyApp.exe hello-word-first-method :p1 "..." :p2 "..." :p 0
#~: MyApp.exe action-with-other-name --p1 "..." --p2 "..." -p 0

default()
default(string a)
default(string a = null)
save(string a = null)
save(int a = 0)
save()


[nada] 
	default()
	
save
	save()
	save(int a = 0)
	default(string a)
	
Não é possível coexistir ações padrão com ações nomeadas, isso ocorre para evitar a situação
	save --p1 "a" -> save(string p1)
	--p1 "a"	  -> default(string p1)
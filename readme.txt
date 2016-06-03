Funcionalidade básica:

Ao criar-se uma classe de comando de ação (que herda de CommandAction), em qualquer lugar do seu projeto console application, todos os seus métodos publicos (e seus paraâmetros) serão habilitados para serem chamados via prompt de comando automaticamente. Contudo, é possível customizar esse processo da seguinte forma:

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
- Métodos publicos com o nome "Default" ou com o atributo "Action(IsDefault=true)" podem ser acessados via prompt sem a necessidade do nome da ação, ou seja, passando apenas os argumentos.

class HelloWordCommand : CommandAction
{
	public HelloWordCommand() {
		this.AddPrefixInActions = true;
		// this.PrefixActions = "custom-prefix";
	}
	
	public void Default(string p1) {
		
	}
	
	[Action(IsDefault=true)]
	public void OtherDefault(string p1) {
		
	}
	
	[Action(Ignore=true)]
	public void IsIgnored(string p1, string p2, int p) {
		
	}
	
	public void FirstMethod(string p1, string p2, int p) {
		
	}
	
	[Action(Name="action-with-other-name", AddPrefix = false)]
	public void SecondMethod
	(
		[Argument(ShortName="p")] string p1,
								  string p2, 
		[Argument(LongName="parameter")] int p
	)
	{
		
	}
}

$~: MyApp.exe default [arguments...]									-> Default(string p1)
$~: MyApp.exe other-default [arguments...]								-> OtherDefault(string p1)
$~: MyApp.exe --p1 "test"												-> Default(string p1)
#~: MyApp.exe hello-word-first-method --p1 "..." --p2 "..." -p 0		-> FirstMethod(string p1, string p2, int p)
#~: MyApp.exe hello-word-first-method :p1 "..." :p2 "..." :p 0			-> FirstMethod(string p1, string p2, int p)
#~: MyApp.exe action-with-other-name --p1 "..." --p2 "..." -p 0			-> SecondMethod(string p1, string p2, int p)

Controle de seleção e execução de ações:

O controle é feito em duas etapas, "seleção" e "execução". Esse controle é feito exclusivamente para executar apenas um método por command e assim conseguir escolher o melhor método em caso de overloads de métodos ou também em mais de um método default no mesmo command. Os passos são:
	
	1) Serão escolhidos os métodos que tiverem as mesmas quantidades de argumentos do que foi enviado no prompt.
	2) Dos métodos escolhidos:
		2.1) Serão selecionados, como canditados a execução, os métodos que tiverem a maior quantidade de nomes de argumentos que combinaram com os nomes dos argumentos que foram enviado.
		2.2) Em caso de haver dois ou mais canditados de execução, o método executado será o que foi definido primeiro.
	3) Em caso de haver dois métodos iguais em commands diferentes, ambos serão executados respeitando a ordem de criação dos commands ou a propriedade "Command.OrderExecution".
	
class HelloWordCommand : CommandAction
{
	public void save(string a) {}
	public void save(int a) {}
	public void save(int a, string b) {}
	public void save(int a, string c) {}
}

class OtherCommand : CommandAction
{
	public void OtherCommand() {
		this.OrderExecution = -1;
	}
	
	public void save(string a) {}
	public void save(int a) {}
	public void save(int a, string b) {}
	public void save(int a, string c) {}
}

Cenário 2.1:

$~: MyApp.exe -a "1" -b "..."

Escolhidos: Ambos os métodos tem a mesma quantidade de argumentos do que foi enviado.	
	OtherCommand.save(int a, string b);
	OtherCommand.save(int a, string c);
	HelloWordCommand.save(int a, string b);
	HelloWordCommand.save(int a, string c);
Executados: Será executado o método que der match com a maior quantidade de argumentos enviados.
	OtherCommand.save(int a, string b);
	HelloWordCommand.save(int a, string b);
	
Cenário 2.2:

$~: MyApp.exe -a "1"

Escolhidos: Ambos os métodos tem a mesma quantidade de argumentos que foi enviado.
	OtherCommand.save(string a);
	OtherCommand.save(int a);
	HelloWordCommand.save(string a);
	HelloWordCommand.save(int a);
Executados: Será executado o primeiro método, pois ele foi definido primeiro.
	OtherCommand.save(string a);
	HelloWordCommand.save(string a);
		
Multiplas ações:

Existe a possíbilidade de em um mesmo input, no prompt de comando, ser executado mais de uma ação. Porém, para que isso seja possível é necessário configurar a propriedade "App.Current.ActionCharPrefix='[algum-char]'" antes do método "App.Current.Run()". Com essa propriedade configurada o comando abaixo teria o seguinte comportamento:

class Program : App
{
	static int Main(string[] args)
	{   
		while (true)
		{
			App.Initialize();
			App.Current.ActionCharPrefix = '$';
			App.Current.Run();
			if (!App.Current.InDebug)
				return App.Current.Response.Code;
		}
	}
}

class HelloWordCommand : CommandAction
{
	public void save(int id) {}
	public void delete(int id) {}
	public void Default(string value) {}
}

$~: MyApp.exe $save --id 1 $delete --id 1 $default --value \$UtilizarBarraParaEscaparOCharSelecionadoDaAction
	HelloWordCommand.save(int id);
	HelloWordCommand.delete(int id);
	HelloWordCommand.default(string value);
		
Argumentos posicionais:

Ao tentar utilizar argumentos posicionais, será feito uma analise interna para tentar organizar as posições enviadas com as posições de cada método/ação e o método será executado em caso de sucesso.

Para multiplas ações:

$~: MyApp.exe $save 1 $delete 1 $default \$UtilizarBarraParaEscaparOCharSelecionadoDaAction
				  [id]        [id]     [value]
				  
Para ações simples:

$~: MyApp.exe default save
				     [value]
				  
Para ações default: É necessário escapar, pois existe uma ação com o nome "save" e com o escape será possível considerar esse input como valor e não como uma ação.

$~: MyApp.exe \save
			 [value]

--------------------------------------------

Outras funcionalidades:

	- É possível criar commands que funcionam apenas em modo de debug no visual studio, isso é útil para criar ações de helper durante o debug como no caso do command nativo "ClearCommand" que limpa o console de debug quando enviado o argumento "--clear". Para isso utilize no construtor do command "OnlyInDebug=true".
	- É possível ignorar um command, mas isso tem que ocorrer antes da linha "App.Run()". Para isso utilize "App.Current.IgnoreCommand<ClearCommand>()".
	- É possível cancelar a continuação da execução de outras ações usando o método "App.Current.StopPropagation()"
	- Sugerimos utilizar o método "App.Current.Response(message, force)" para exibir informações no console. Ele esta fortemente ligado aos parametros "--verbose, -v" e "--quiet" do command nativo "VerboseCommand". O "verbose" pode determinar níveis do tipo de mensagem que pode ou não ser exibido e o "quiet" sobrepõe o "verbose" não exibindo nenhuma informação, mesmo quando "force=true".
		--verbose all,none,info,success,warning,critical,error,question
		--quiet true|false|0|1
	- Por default o command nativo "HelperCommand" dá a possíbilidade de exibir ajuda utilizando o argumento "--help, -?"
	
--------------------------------------------
Não é possível coexistir ações padrão com ações nomeadas, isso ocorre para evitar a situação
	save --p1 "a" -> save(string p1)
	--p1 "a"	  -> default(string p1)
# Web Scraping

## Visão Geral
Este projeto consiste em uma aplicação de web scraping que utiliza o HtmlAgilityPack para extrair dados de páginas da web, armazenando esses dados em um banco de dados MySQL. As informações são organizadas em duas classes principais: Product e ProductSpec. O Dapper é utilizado para a comunicação com o banco de dados, facilitando a inserção, atualização e recuperação dos dados. A interface do usuário é construída em um aplicativo Windows Forms, com a exibição dos dados feita através de DataGridViews e botões de edição para manipulação dos registros.
- Site do scraping: https://www.tbca.net.br/base-dados/composicao_estatistica.php?pagina=1&atuald=1#

## Demonstrativo (vídeo)
<a hfer="https://www.linkedin.com/posts/igor-barcelo-631010216_webscraping-dotnet-datascience-activity-7186848754765815809-JiLw?utm_source=share&utm_medium=member_desktop&rcm=ACoAADZ2dIUBHlgJEC2FKQkSO200hlfncqexis4">Link</a>
<div data-badges>
📸 <img src="https://github.com/IgorBarcelo/WebScraping/blob/main/public/demo.png?raw=true)](" width="350" />
</div>

## Pré-requisitos de Instalação
- Você deve ter instalado na máquina o MySQL 8.0 ou superior.
- A senha default no código é 1234 e o usuário root. Caso seja diferente, os dados devem ser alterados no código.

## Instalação
1. Clone o repositório: `git clone https://github.com/IgorBarcelo/WebScraping.git`
2. Abra o projeto em sua IDE preferida.
3. Certifique-se de ter todas as dependências instaladas. Você pode instalar as dependências do projeto através do NuGet Package Manager ou diretamente editando o arquivo .csproj.
4. Configure a conexão com o banco de dados MySQL no arquivo app.config.

## Uso
- Execute o projeto.
- A aplicação carregará os dados da web e exibirá os resultados em DataGridViews.
- Use os botões de edição para atualizar, excluir ou manipular os registros conforme necessário.

## Tecnologias e Ferramentas Utilizadas
- C#
- Windows Forms
- HtmlAgilityPack
- Dapper
- MySQL
- Git

## Avisos
Na pasta Database está o banco ja carregado caso nao queira esperar o processo de preencher a tabela do zero, é só descompactar o arquivo webscraping.rar nesse caminho C:\ProgramData\MySQL\MySQL Server 8.0\Data pode variar dependendo da versão do MySLQ, localizando a pasta e descompactando o arquivo (caso nesessário reinicie o mysql) e o programa já reconhecerá o banco.

## Créditos
Desenvolvido por [Igor Barcelo](https://github.com/IgorBarcelo)

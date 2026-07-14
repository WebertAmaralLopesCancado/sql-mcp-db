# SQL MCP DB

O **SQL MCP DB** é um servidor [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) desenvolvido em .NET para consultar e explorar bancos de dados Microsoft SQL Server por meio de clientes compatíveis com MCP, como o Codex.

O servidor utiliza o transporte `stdio` e disponibiliza ferramentas para inspecionar a estrutura do banco, consultar relacionamentos e executar consultas somente leitura.

## Funcionalidades

As ferramentas MCP disponíveis permitem:

- consultar as informações da conexão atual sem expor a senha;
- obter o nome do banco conectado;
- listar tabelas, schemas e quantidade de colunas;
- pesquisar tabelas e colunas por nome;
- descrever as colunas de uma tabela;
- consultar relacionamentos e chaves estrangeiras;
- executar consultas iniciadas por `SELECT` ou `WITH`.

## Pré-requisitos

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) ou superior compatível;
- acesso a uma instância do Microsoft SQL Server;
- usuário do banco com permissão de leitura nos objetos que serão consultados;
- um cliente compatível com MCP.

## Configuração da conexão

Para o servidor funcionar, é obrigatório preencher a connection string `SqlServer` no arquivo `sql-mcp-db/appsettings.json`.

Crie o arquivo, caso ainda não exista, usando a seguinte estrutura:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=SEU_SERVIDOR;Database=SEU_BANCO;User Id=SEU_USUARIO;Password=SUA_SENHA;Encrypt=True;TrustServerCertificate=True;"
  }
}
```

Para autenticação integrada do Windows, um exemplo é:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=SEU_SERVIDOR;Database=SEU_BANCO;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"
  }
}
```

Substitua os valores de exemplo pelos dados do ambiente. Não publique senhas ou outras credenciais reais no repositório.

## Compilação e execução

Na raiz do repositório, execute:

```powershell
dotnet restore .\sql-mcp-db\sql-mcp-db.csproj
dotnet build .\sql-mcp-db\sql-mcp-db.csproj
dotnet run --project .\sql-mcp-db\sql-mcp-db.csproj
```

Como o servidor se comunica por `stdio`, é normal que ele permaneça em execução sem apresentar uma interface ou menu no terminal.

## Configuração no Codex

Para registrar o servidor MCP globalmente no Codex, execute na raiz do repositório:

```powershell
codex mcp add sql-mcp-db -- dotnet run --project C:\GIT\sql-mcp-db\sql-mcp-db
```

Se o repositório estiver em outro diretório, ajuste o caminho do projeto no comando. Depois do cadastro, reinicie o Codex para carregar a configuração.

## Segurança

A ferramenta de consulta livre aceita apenas comandos cujo primeiro termo seja `SELECT` ou `WITH`. Ainda assim, recomenda-se usar uma conta SQL dedicada e com permissões exclusivamente de leitura, aplicando o princípio do menor privilégio no próprio banco de dados.

## Estrutura do projeto

```text
sql-mcp-db/
├── Models/                 # Modelos retornados pelas consultas
├── Services/               # Acesso e análise do SQL Server
├── Tools/                  # Ferramentas expostas pelo servidor MCP
├── appsettings.json        # Configuração local da conexão
├── Program.cs              # Inicialização do servidor MCP
└── sql-mcp-db.csproj       # Projeto .NET
```

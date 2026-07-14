using ModelContextProtocol.Server;
using SqlMcpDb.Services;
using System.ComponentModel;

namespace SqlMcpDb.Tools;

[McpServerToolType]
public sealed class DatabaseMcpTools
{
    private readonly DatabaseAnalyzer _analyzer;

    public DatabaseMcpTools(DatabaseAnalyzer analyzer)
    {
        _analyzer = analyzer;
    }

[McpServerTool]
[Description("Retorna informações da string de conexão atual, ocultando a senha.")]
public async Task<object> GetCurrentConnectionInfo()
{
    return await _analyzer.GetCurrentConnectionInfoAsync();
}

[McpServerTool]
[Description("Retorna o nome do banco SQL Server atualmente conectado.")]
public async Task<string> GetCurrentDatabase()
{
    return await _analyzer.GetCurrentDatabaseAsync();
}


    [McpServerTool]
    [Description("Retorna uma visão geral do banco de dados.")]
    public async Task<object> GetDatabaseOverview()
    {
        return await _analyzer.GetDatabaseOverviewAsync();
    }

    [McpServerTool]
    [Description("Procura colunas pelo nome.")]
    public async Task<object> SearchColumns(
        string searchTerm)
    {
        return await _analyzer.SearchColumnsAsync(
            searchTerm);
    }

    [McpServerTool]
    [Description("Descreve a estrutura de uma tabela.")]
    public async Task<object> DescribeTable(
        string schema,
        string table)
    {
        return await _analyzer.DescribeTableAsync(
            schema,
            table);
    }

    [McpServerTool]
    [Description("Obtém relacionamentos entre tabelas.")]
    public async Task<object> GetRelationships()
    {
        return await _analyzer.GetRelationshipsAsync();
    }

    [McpServerTool]
    [Description("Executa uma consulta somente leitura.")]
    public async Task<object> ExecuteReadonlyQuery(
        string sql)
    {
        return await _analyzer.ExecuteReadonlyQueryAsync(
            sql);
    }
}

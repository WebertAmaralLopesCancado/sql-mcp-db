namespace SqlMcpDb.Models;

public sealed class QueryResult
{
    public List<string> Columns { get; set; } = [];

    public List<Dictionary<string, object?>> Rows { get; set; } = [];
}

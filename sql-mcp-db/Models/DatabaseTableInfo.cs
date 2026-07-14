namespace SqlMcpDb.Models;

public sealed class DatabaseTableInfo
{
    public string Schema { get; set; } = string.Empty;

    public string Table { get; set; } = string.Empty;

    public int ColumnCount { get; set; }
}

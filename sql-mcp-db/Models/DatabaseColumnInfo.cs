namespace SqlMcpDb.Models;

public sealed class DatabaseColumnInfo
{
    public string Schema { get; set; } = string.Empty;

    public string Table { get; set; } = string.Empty;

    public string Column { get; set; } = string.Empty;

    public string DataType { get; set; } = string.Empty;

    public int MaxLength { get; set; }

    public bool IsNullable { get; set; }
}

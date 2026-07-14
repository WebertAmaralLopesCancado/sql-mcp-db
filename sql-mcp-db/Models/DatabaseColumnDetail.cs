namespace SqlMcpDb.Models;

public sealed class DatabaseColumnDetail
{
    public string Column { get; set; } = string.Empty;

    public string DataType { get; set; } = string.Empty;

    public int MaxLength { get; set; }

    public bool IsNullable { get; set; }

    public bool IsIdentity { get; set; }
}

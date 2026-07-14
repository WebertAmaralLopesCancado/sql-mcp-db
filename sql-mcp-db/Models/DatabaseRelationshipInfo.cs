namespace SqlMcpDb.Models;

public sealed class DatabaseRelationshipInfo
{
    public string ParentSchema { get; set; } = string.Empty;

    public string ParentTable { get; set; } = string.Empty;

    public string ParentColumn { get; set; } = string.Empty;

    public string ReferencedSchema { get; set; } = string.Empty;

    public string ReferencedTable { get; set; } = string.Empty;

    public string ReferencedColumn { get; set; } = string.Empty;

    public string ForeignKeyName { get; set; } = string.Empty;
}

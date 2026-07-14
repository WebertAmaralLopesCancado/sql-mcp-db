using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SqlMcpDb.Models;

namespace SqlMcpDb.Services;

public sealed class DatabaseAnalyzer
{
    private readonly string _connectionString;

    public DatabaseAnalyzer(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Connection string 'SqlServer' não encontrada.");
    }

    private SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public async Task<object> GetCurrentConnectionInfoAsync()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString);

        await using var connection = CreateConnection();
        await connection.OpenAsync();

        return new
        {
            Server = builder.DataSource,
            ConfiguredDatabase = builder.InitialCatalog,
            ConnectedDatabase = connection.Database,
            User = builder.UserID,
            Encrypt = builder.Encrypt,
            TrustServerCertificate = builder.TrustServerCertificate,
            ConnectionTimeout = builder.ConnectTimeout
        };
    }

    public async Task<string> GetCurrentDatabaseAsync()
    {
        await using var connection = CreateConnection();

        await connection.OpenAsync();

        return connection.Database;
    }

    public async Task<List<DatabaseTableInfo>> GetDatabaseOverviewAsync()
    {
        var result = new List<DatabaseTableInfo>();

        const string sql = """
            SELECT
                s.name AS SchemaName,
                t.name AS TableName,
                COUNT(c.column_id) AS ColumnCount
            FROM sys.tables t
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            LEFT JOIN sys.columns c ON t.object_id = c.object_id
            GROUP BY s.name, t.name
            ORDER BY s.name, t.name
            """;

        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new DatabaseTableInfo
            {
                Schema = reader["SchemaName"].ToString()!,
                Table = reader["TableName"].ToString()!,
                ColumnCount = Convert.ToInt32(reader["ColumnCount"])
            });
        }

        return result;
    }

    public async Task<List<DatabaseColumnInfo>> SearchColumnsAsync(string term)
    {
        var result = new List<DatabaseColumnInfo>();

        const string sql = """
            SELECT
                s.name AS SchemaName,
                t.name AS TableName,
                c.name AS ColumnName,
                ty.name AS DataType,
                c.max_length AS MaxLength,
                c.is_nullable AS IsNullable
            FROM sys.columns c
            INNER JOIN sys.tables t ON c.object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
            WHERE c.name LIKE @term
               OR t.name LIKE @term
               OR s.name LIKE @term
            ORDER BY s.name, t.name, c.name
            """;

        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@term", $"%{term}%");

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new DatabaseColumnInfo
            {
                Schema = reader["SchemaName"].ToString()!,
                Table = reader["TableName"].ToString()!,
                Column = reader["ColumnName"].ToString()!,
                DataType = reader["DataType"].ToString()!,
                MaxLength = Convert.ToInt32(reader["MaxLength"]),
                IsNullable = Convert.ToBoolean(reader["IsNullable"])
            });
        }

        return result;
    }

    public async Task<List<DatabaseColumnDetail>> DescribeTableAsync(string schema, string table)
    {
        var result = new List<DatabaseColumnDetail>();

        const string sql = """
            SELECT
                c.name AS ColumnName,
                ty.name AS DataType,
                c.max_length AS MaxLength,
                c.is_nullable AS IsNullable,
                c.is_identity AS IsIdentity
            FROM sys.columns c
            INNER JOIN sys.tables t ON c.object_id = t.object_id
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
            WHERE s.name = @schema
              AND t.name = @table
            ORDER BY c.column_id
            """;

        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@schema", schema);
        command.Parameters.AddWithValue("@table", table);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new DatabaseColumnDetail
            {
                Column = reader["ColumnName"].ToString()!,
                DataType = reader["DataType"].ToString()!,
                MaxLength = Convert.ToInt32(reader["MaxLength"]),
                IsNullable = Convert.ToBoolean(reader["IsNullable"]),
                IsIdentity = Convert.ToBoolean(reader["IsIdentity"])
            });
        }

        return result;
    }

    public async Task<List<DatabaseRelationshipInfo>> GetRelationshipsAsync()
    {
        var result = new List<DatabaseRelationshipInfo>();

        const string sql = """
            SELECT
                parentSchema.name AS ParentSchema,
                parentTable.name AS ParentTable,
                parentColumn.name AS ParentColumn,
                referencedSchema.name AS ReferencedSchema,
                referencedTable.name AS ReferencedTable,
                referencedColumn.name AS ReferencedColumn,
                fk.name AS ForeignKeyName
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
            INNER JOIN sys.tables parentTable ON fkc.parent_object_id = parentTable.object_id
            INNER JOIN sys.schemas parentSchema ON parentTable.schema_id = parentSchema.schema_id
            INNER JOIN sys.columns parentColumn ON fkc.parent_object_id = parentColumn.object_id
                AND fkc.parent_column_id = parentColumn.column_id
            INNER JOIN sys.tables referencedTable ON fkc.referenced_object_id = referencedTable.object_id
            INNER JOIN sys.schemas referencedSchema ON referencedTable.schema_id = referencedSchema.schema_id
            INNER JOIN sys.columns referencedColumn ON fkc.referenced_object_id = referencedColumn.object_id
                AND fkc.referenced_column_id = referencedColumn.column_id
            ORDER BY parentSchema.name, parentTable.name, fk.name
            """;

        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new DatabaseRelationshipInfo
            {
                ParentSchema = reader["ParentSchema"].ToString()!,
                ParentTable = reader["ParentTable"].ToString()!,
                ParentColumn = reader["ParentColumn"].ToString()!,
                ReferencedSchema = reader["ReferencedSchema"].ToString()!,
                ReferencedTable = reader["ReferencedTable"].ToString()!,
                ReferencedColumn = reader["ReferencedColumn"].ToString()!,
                ForeignKeyName = reader["ForeignKeyName"].ToString()!
            });
        }

        return result;
    }

    public async Task<QueryResult> ExecuteReadonlyQueryAsync(string sql)
    {
        if (!IsReadonlyQuery(sql))
        {
            throw new InvalidOperationException(
                "Query bloqueada. Apenas comandos SELECT ou WITH são permitidos.");
        }

        var result = new QueryResult();

        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        for (var i = 0; i < reader.FieldCount; i++)
        {
            result.Columns.Add(reader.GetName(i));
        }

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = await reader.IsDBNullAsync(i)
                    ? null
                    : reader.GetValue(i);
            }

            result.Rows.Add(row);
        }

        return result;
    }

    private static bool IsReadonlyQuery(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return false;

        var firstCommand = sql
            .Trim()
            .Split(' ', '\r', '\n', '\t')
            .First()
            .ToUpperInvariant();

        return firstCommand is "SELECT" or "WITH";
    }
}

using System.Collections;
using System.Collections.Immutable;
using System.Data;

using Library.Data.SqlServer.Dynamics.Collections;

using Microsoft.Data.SqlClient;

using static Library.Data.SqlServer.SqlStatementBuilder;

namespace Library.Data.SqlServer.Dynamics;

public sealed class Table(Database owner, string name, string? schema = null, string? connectionString = null) : SqlObject<Table, Database>(owner, name, schema, connectionString ?? owner.ConnectionString), IEnumerable
{
    private Columns? _columns;

    public Columns Columns { get => this._columns ??= this.GetColumns(); set => this._columns = value; }
    public DateTime CreateDate { get; set; }
    public long Id { get; set; }
    public DateTime ModifyDate { get; set; }

    public IEnumerable<Row> Rows
    {
        get
        {
            using var reader = this.ToSqlDataReader();
            while (reader.Read())
            {
                yield return new Row(this, this.Columns.Select(c => new KeyValuePair<string, object?>(c.Name, reader[c.Name])).ToDictionary());
            }
        }
    }

    public Row this[int index] => this.Rows.ElementAt(index);

    public static Tables? GetByConnectionString(string connectionString) =>
        Database.GetDatabase(connectionString)?.Tables;

    public static ImmutableList<Column> GetTableColumns(Table owner)
    {
        var qColumns = string.Format(QueryBank.COLUMNS_WHERE_TABLE_NAME, owner.Name);
        var qIdentities = string.Format(QueryBank.IDENTITIES_WHERE_TABLE_NAME, owner.Name);
        var qForeignKeys = string.Format(QueryBank.FOREIGN_KEY_COLUMNS_WHERE_TABLE_NAME, owner.Name);
        var query = new[] { qColumns, qIdentities, qForeignKeys }.Merge("; ");
        var result = new List<Column>();
        using var conn = new SqlConnection(owner.ConnectionString);
        using var reader = conn.ExecuteReader(query);
        while (reader.Read())
        {
            var column = new Column(default!, reader.Field("name", Convert.ToString)!, owner.ConnectionString)
            {
                CollationName = reader.Field("collation_name", Convert.ToString)!,
                IsNullable = reader.Field("is_nullable", str => str.ToString()!.EqualsTo("True")),
                MaxLength = reader.Field("max_length", v => DBNull.Value.Equals(v) ? 0 : Convert.ToInt32(v)),
                Precision = reader.Field("precision", v => DBNull.Value.Equals(v) ? 0 : Convert.ToInt32(v)),
                Position = reader.Field("column_id", v => DBNull.Value.Equals(v) ? 0 : Convert.ToInt32(v)),
                DataType = reader.Field("system_type", Convert.ToString)!,
                UniqueId = string.Concat(reader["object_id"].Cast().ToLong().ToString("000000000000"), reader["column_id"].Cast().ToInt().ToString("000")).Cast().ToLong(),
            };
            result.Add(column);
        }

        _ = reader.NextResult();
        while (reader.Read())
        {
            result.First(c => c.Name == reader.Field("column_name", Convert.ToString)).IsIdentity = true;
        }

        _ = reader.NextResult();
        while (reader.Read())
        {
            var column = result.First(c => c.Name == reader.Field("ParentColumn", Convert.ToString));
            column.IsForeignKey = true;
            column.ForeignKeyInfo = new ForeignKeyInfo
            {
                ReferencedColumn = reader.Field("ReferencedColumn", Convert.ToString),
                ReferencedTable = reader.Field("ReferencedTable", Convert.ToString)
            };
        }
        return result.ToImmutableList();
    }

    public IEnumerator GetEnumerator()
    {
        using var dataTable = this.ToDataTable(this.Columns.Select(col => col.Name).ToArray());
        return dataTable.Rows.GetEnumerator();
    }

    public IEnumerable<string> PrintFormatted(CancellationTokenSource? cancellation = null,
        string columnsSeparator = "\t",
        string cellsSeparator = "\t",
        params string[] columns)
    {
        if (isCancelled())
        {
            yield break;
        }
        var table = this.ToDataTable(columns);
        if (columns?.Any() is not true)
        {
            columns = this.Columns.Select(c => c.Name).ToArray();
        }

        if (isCancelled())
        {
            yield break;
        }

        var lengths = new Dictionary<string, int>();
        foreach (var column in columns)
        {
            if (isCancelled())
            {
                yield break;
            }

            var max = 0;
            foreach (DataRow row in table.Rows)
            {
                if (isCancelled())
                {
                    yield break;
                }

                var length = row[column]?.ToString()?.Length;
                if (length > max)
                {
                    max = length.Value;
                }

                if (column.Length > max)
                {
                    max = column.Length;
                }
            }

            lengths.Add(column, max);
        }

        for (var index = 0; index < columns.Length; index++)
        {
            if (isCancelled())
            {
                yield break;
            }

            var column = columns[index];
            if (column.Length < lengths[column])
            {
                var colLen = lengths[column] - column.Length;
                var col = column.Add(colLen / 2, before: true)!.Add(colLen / 2)!.AddToEnd(" ");
                yield return col;
            }
            else
            {
                yield return column;
            }

            if (index != columns.Length - 1)
            {
                yield return columnsSeparator;
            }
        }

        yield return Environment.NewLine;
        foreach (DataRow row in table.Rows)
        {
            if (isCancelled())
            {
                yield break;
            }

            for (var index = 0; index < columns.Length; index++)
            {
                if (isCancelled())
                {
                    yield break;
                }

                var column = columns[index];
                var data = row[column]?.ToString() ?? string.Empty;
                if (data.Length < lengths[column])
                {
                    data = data.Add(lengths[column] - data.Length);
                }

                yield return data!;
                if (index != columns.Length - 1)
                {
                    yield return cellsSeparator;
                }
            }

            yield return Environment.NewLine;
        }
        bool isCancelled() => cancellation?.IsCancellationRequested ?? false;
    }

    public IEnumerable<Row> Select(params string[] columns)
    {
        if (!columns.Any())
        {
            columns = this.Columns.Select(c => c.Name).ToArray();
        }

        using var reader = this.ToReader(columns);
        while (reader.Read())
        {
            yield return new Row(this, columns.Select(column => new KeyValuePair<string, object>(column, reader[column])).ToList(),
                this.ConnectionString);
        }
    }

    public DataTable ToDataTable(params string[] columns)
    {
        using var connection = new SqlConnection(this.ConnectionString);
        return connection.FillDataTable(CreateSelect(this.ToString(), columns));
    }

    public DataTable ToDataTableWhere([DisallowNull] string condition, params string[] columns)
    {
        using var conn = new SqlConnection(this.ConnectionString);
        return conn.FillDataTable($"{CreateSelect(this.Name, columns)} WHERE {condition}");
    }

    public DataReader ToReader(params string[] columns)
    {
        var connection = new SqlConnection(this.ConnectionString);
        return new(connection.ExecuteReader(CreateSelect(this.Name, columns)), this.Owner, this.Name, this.ConnectionString);
    }

    public SqlDataReader ToReaderWhere(string condition, params string[] columns)
    {
        var connection = new SqlConnection(connectionString);
        return connection.ExecuteReader($"{CreateSelect(this.Name, columns)} WHERE {condition}");
    }

    public SqlDataReader ToSqlDataReader(params string[] columns)
    {
        var connection = new SqlConnection(connectionString);
        return connection.ExecuteReader(CreateSelect(this.ToString(), columns));
    }

    public SqlDataReader Where(string condition, params string[] columns)
    {
        var connection = new SqlConnection(connectionString);
        return connection.ExecuteReader($"{CreateSelect(this.Name, columns)} WHERE {condition}");
    }

    private Columns GetColumns()
    {
        var columns = GetTableColumns(this);
        return new Columns(columns);
    }

    //private Columns GetColumns()
    //{
    //    var helper = new Sql(this.ConnectionString);
    //    var qColumns = string.Format(QueryBank.COLUMNS_WHERE_TABLE_NAME, this.Name);
    //    var qIdentities = string.Format(QueryBank.IDENTITIES_WHERE_TABLE_NAME, this.Name);
    //    var qForeignKeys = string.Format(QueryBank.FOREIGN_KEY_COLUMNS_WHERE_TABLE_NAME, this.Name);
    //    var query = new[] { qColumns, qIdentities, qForeignKeys }.Merge("; ");
    //    var columns = new List<Column>();

    // var stopwatch = Stopwatch.StartNew(); byReader(helper, query, columns); stopwatch.Stop(); var
    // t2 = stopwatch.Elapsed;

    // stopwatch.Restart(); byAdapter(helper, qColumns, qIdentities, qForeignKeys);
    // stopwatch.Stop(); var t1 = stopwatch.Elapsed;

    // var faster = t2 > t1 ? "Adapter" : "Reader"; return new Columns(columns);

    // static void byAdapter(Sql helper, string qColumns, string qIdentities, string qForeignKeys) {
    // var tables = helper.FillDataTables(qColumns, qIdentities, qForeignKeys).ToArray(); var
    // tColumns = tables[0]; var tIdentities = tables[1]; var tForeignKeys = tables[2]; }

    // static void byReader(Sql helper, string query, List<Column> columns) { using (var reader =
    // helper.ExecuteReader(query)) { while (reader.Read()) { columns.Add(new Column(null,
    // reader.Field("name", Convert.ToString), null) { CollationName =
    // reader.Field("collation_name", Convert.ToString), IsNullable = reader.Field("is_nullable",
    // str => str.ToString().EqualsTo("1")), MaxLength = reader.Field("max_length", v =>
    // DBNull.Value.Equals(v) ? 0 : Convert.ToInt32(v)), Precision = reader.Field("precision", v =>
    // DBNull.Value.Equals(v) ? 0 : Convert.ToInt32(v)), Position = reader.Field("column_id", v =>
    // DBNull.Value.Equals(v) ? 0 : Convert.ToInt32(v)), DataType = reader.Field("system_type",
    // Convert.ToString), UniqueId =
    // string.Concat(reader["object_id"].ToLong().ToString("000000000000"),
    // reader["column_id"].ToInt().ToString("000")).ToLong(), }); }

    // _ = reader.NextResult(); while (reader.Read()) { columns.First(c => c.Name ==
    // reader.Field("column_name", Convert.ToString)).IsIdentity = true; }

    //            _ = reader.NextResult();
    //            while (reader.Read())
    //            {
    //                var column = columns.First(c => c.Name == reader.Field("ParentColumn", Convert.ToString));
    //                column.IsForeignKey = true;
    //                column.ForeignKeyInfo = new ForeignKeyInfo
    //                {
    //                    ReferencedColumn = reader.Field("ReferencedColumn", Convert.ToString),
    //                    ReferencedTable = reader.Field("ReferencedTable", Convert.ToString)
    //                };
    //            }
    //        }
    //    }
    //}
}
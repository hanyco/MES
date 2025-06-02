using System.Data;
using Microsoft.Data.SqlClient;

using Library.Data.SqlServer.Dynamics.Collections;

namespace Library.Data.SqlServer.Dynamics;

public sealed class StoredProcedure(Database owner, string name, string schema, string connectionString) : SqlObject<StoredProcedure, Database>(owner, name, schema, connectionString)
{
    private StoredProcedureParams _params;

    public string AssemblyName { get; set; }
    public string Body { get; set; }
    public DateTime CreateDate { get; set; }
    public long Id { get; set; }
    public bool IsSystemObject { get; set; }
    public DateTime LastModifiedDate { get; set; }

    public StoredProcedureParams Params =>
        this._params ??= new StoredProcedureParams(from row in
                                                       (from row in this.Owner.AllParams
                                                        where row.Field<string>("SpName").EqualsTo(this.Name)
                                                        select row).ToList()
                                                   select new StoredProcedureParam(this, row.Field<string>("name"), this.ConnectionString)
                                                   {
                                                       DefaultValue = row.Field("DefaultValue", Convert.ToString),
                                                       Id = row.Field("ID", Convert.ToInt64),
                                                       Length = row.Field("Length", Convert.ToInt32),
                                                       NumericPrecision = row.Field("NumericPrecision", Convert.ToInt32),
                                                       SqlDataType = row.Field("DataType", Convert.ToString)
                                                   });

    public new string Schema { get; set; }

    public bool Run(params KeyValuePair<string, object>[] args) => true;

    public bool TryGatherResultSet(out Dictionary<string, string> prms, Action<Exception>? exceptionHandler = null)
    {
        bool flag;
        prms = new Dictionary<string, string>();
        using (var connection = new SqlConnection(this.Owner.ConnectionString))
        using (var command = connection.CreateCommand())
        {
            var builder = new StringBuilder("SET FMTONLY OFF; SET FMTONLY ON;");
            _ = builder.AppendLine();
            _ = builder.Append("exec");
            _ = builder.AppendFormat(" {0}.{1}.{2}", this.Owner.Name, this.Schema, this.Name);
            if (this.Params.Any())
            {
                _ = builder.AppendLine();
                foreach (var param in this.Params)
                {
                    _ = builder.AppendFormat(" {0}=NULL, ", param.Name);
                }

                _ = builder.Remove(builder.Length - 2, 2);
            }

            _ = builder.AppendLine();
            _ = builder.AppendLine("SET FMTONLY OFF;");
            command.CommandText = builder.ToString();
            try
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.FieldCount > 0)
                    {
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            prms.Add(reader.GetName(i), reader.GetFieldType(i).ToString());
                        }
                    }
                }

                flag = true;
            }
            catch (Exception exception)
            {
                exceptionHandler?.Invoke(exception);
                flag = false;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        return flag;
    }
}
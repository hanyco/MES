using Microsoft.Data.SqlClient;

namespace DataLib;

internal interface IConnectionFactory
{
    SqlConnection CreateConnection();
}
using Oracle.ManagedDataAccess.Client;

namespace StajApi.Data;

public interface IDbConnectionFactory
{
    OracleConnection CreateConnection();
}

using Oracle.ManagedDataAccess.Client;

namespace StajApi.Data;

public class OracleConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public OracleConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public OracleConnection CreateConnection()
    {
        var connectionString =
            _configuration.GetConnectionString("OracleDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "OracleDb bağlantı bilgisi bulunamadı.");
        }

        return new OracleConnection(connectionString);
    }
}

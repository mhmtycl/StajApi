using Oracle.ManagedDataAccess.Client;
using StajApi.Data;

namespace StajApi.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RefreshTokenRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task SaveAsync(string username, string refreshToken)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.BindByName = true;

        command.CommandText = """
            INSERT INTO REFRESH_TOKENS
                (USERNAME, TOKEN, EXPIRES_AT, IS_REVOKED)
            VALUES
                (:username, :token, :expiresAt, 0)
            """;

        command.Parameters.Add(
            "username",
            OracleDbType.Varchar2).Value = username;

        command.Parameters.Add(
            "token",
            OracleDbType.Varchar2).Value = refreshToken;

        command.Parameters.Add(
            "expiresAt",
            OracleDbType.TimeStamp).Value =
            DateTime.UtcNow.AddDays(7);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<string?> GetActiveUsernameAsync(string refreshToken)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.BindByName = true;

        command.CommandText = @"
            SELECT USERNAME
            FROM REFRESH_TOKENS
            WHERE TOKEN = :token
              AND IS_REVOKED = 0
              AND EXPIRES_AT > SYSTIMESTAMP";

        command.Parameters.Add(
            "token",
            OracleDbType.Varchar2).Value = refreshToken;

        var result = await command.ExecuteScalarAsync();

        return result as string;
    }

    public async Task RevokeAsync(string refreshToken)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.BindByName = true;

        command.CommandText = """
            UPDATE REFRESH_TOKENS
            SET IS_REVOKED = 1
            WHERE TOKEN = :token
            """;

        command.Parameters.Add(
            "token",
            OracleDbType.Varchar2).Value = refreshToken;

        await command.ExecuteNonQueryAsync();
    }
}

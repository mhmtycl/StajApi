using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Calisanlar;

public class DeleteCalisanCommandHandler
    : ICommandHandler<DeleteCalisanCommand, bool>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DeleteCalisanCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> Handle(DeleteCalisanCommand command)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var deleteCommand = connection.CreateCommand();
        deleteCommand.BindByName = true;

        deleteCommand.CommandText = """
            DELETE FROM CALISANLAR
            WHERE CALISAN_ID = :calisanId
            """;

        deleteCommand.Parameters.Add(
            "calisanId",
            OracleDbType.Int32).Value = command.CalisanId;

        var etkilenen = await deleteCommand.ExecuteNonQueryAsync();

        return etkilenen > 0;
    }
}

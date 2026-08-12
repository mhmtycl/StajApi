using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Yoneticiler;

public class DeleteYoneticiCommandHandler
    : ICommandHandler<DeleteYoneticiCommand, bool>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DeleteYoneticiCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> Handle(DeleteYoneticiCommand command)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var deleteCommand = connection.CreateCommand();
        deleteCommand.BindByName = true;

        deleteCommand.CommandText = """
            DELETE FROM YONETICILER
            WHERE YONETICI_ID = :yoneticiId
            """;

        deleteCommand.Parameters.Add(
            "yoneticiId",
            OracleDbType.Int32).Value = command.YoneticiId;

        var etkilenen = await deleteCommand.ExecuteNonQueryAsync();

        return etkilenen > 0;
    }
}

using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Yoneticiler;

public class UpdateYoneticiCommandHandler
    : ICommandHandler<UpdateYoneticiCommand, bool>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UpdateYoneticiCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> Handle(UpdateYoneticiCommand command)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var updateCommand = connection.CreateCommand();
        updateCommand.BindByName = true;

        updateCommand.CommandText = """
            UPDATE YONETICILER
            SET AD = :ad,
                SOYAD = :soyad,
                DEPARTMAN = :departman
            WHERE YONETICI_ID = :yoneticiId
            """;

        updateCommand.Parameters.Add(
            "ad",
            OracleDbType.Varchar2).Value = command.Ad;

        updateCommand.Parameters.Add(
            "soyad",
            OracleDbType.Varchar2).Value = command.Soyad;

        updateCommand.Parameters.Add(
            "departman",
            OracleDbType.Varchar2).Value = command.Departman;

        updateCommand.Parameters.Add(
            "yoneticiId",
            OracleDbType.Int32).Value = command.YoneticiId;

        var etkilenen = await updateCommand.ExecuteNonQueryAsync();

        return etkilenen > 0;
    }
}

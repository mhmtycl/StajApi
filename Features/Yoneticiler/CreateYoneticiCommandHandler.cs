using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Yoneticiler;

public class CreateYoneticiCommandHandler
    : ICommandHandler<CreateYoneticiCommand, int>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CreateYoneticiCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> Handle(CreateYoneticiCommand command)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var idCommand = connection.CreateCommand();
        idCommand.CommandText =
            "SELECT NVL(MAX(YONETICI_ID), 0) + 1 FROM YONETICILER";

        var yeniId = Convert.ToInt32(await idCommand.ExecuteScalarAsync());

        await using var insertCommand = connection.CreateCommand();
        insertCommand.BindByName = true;

        insertCommand.CommandText = """
            INSERT INTO YONETICILER
                (YONETICI_ID, AD, SOYAD, DEPARTMAN)
            VALUES
                (:yoneticiId, :ad, :soyad, :departman)
            """;

        insertCommand.Parameters.Add(
            "yoneticiId",
            OracleDbType.Int32).Value = yeniId;

        insertCommand.Parameters.Add(
            "ad",
            OracleDbType.Varchar2).Value = command.Ad;

        insertCommand.Parameters.Add(
            "soyad",
            OracleDbType.Varchar2).Value = command.Soyad;

        insertCommand.Parameters.Add(
            "departman",
            OracleDbType.Varchar2).Value = command.Departman;

        await insertCommand.ExecuteNonQueryAsync();

        return yeniId;
    }
}

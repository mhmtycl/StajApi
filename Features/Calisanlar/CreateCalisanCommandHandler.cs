using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Calisanlar;

public class CreateCalisanCommandHandler
    : ICommandHandler<CreateCalisanCommand, int>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CreateCalisanCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> Handle(CreateCalisanCommand command)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var idCommand = connection.CreateCommand();
        idCommand.CommandText =
            "SELECT NVL(MAX(CALISAN_ID), 0) + 1 FROM CALISANLAR";

        var yeniId = Convert.ToInt32(await idCommand.ExecuteScalarAsync());

        await using var insertCommand = connection.CreateCommand();
        insertCommand.BindByName = true;

        insertCommand.CommandText = """
            INSERT INTO CALISANLAR
                (CALISAN_ID, AD, SOYAD, MAAS)
            VALUES
                (:calisanId, :ad, :soyad, :maas)
            """;

        insertCommand.Parameters.Add(
            "calisanId",
            OracleDbType.Int32).Value = yeniId;

        insertCommand.Parameters.Add(
            "ad",
            OracleDbType.Varchar2).Value = command.Ad;

        insertCommand.Parameters.Add(
            "soyad",
            OracleDbType.Varchar2).Value = command.Soyad;

        insertCommand.Parameters.Add(
            "maas",
            OracleDbType.Decimal).Value = command.Maas;

        await insertCommand.ExecuteNonQueryAsync();

        return yeniId;
    }
}

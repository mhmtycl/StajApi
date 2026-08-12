using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Calisanlar;

public class UpdateCalisanCommandHandler
    : ICommandHandler<UpdateCalisanCommand, bool>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UpdateCalisanCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> Handle(UpdateCalisanCommand command)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var updateCommand = connection.CreateCommand();
        updateCommand.BindByName = true;

        updateCommand.CommandText = """
            UPDATE CALISANLAR
            SET AD = :ad,
                SOYAD = :soyad,
                MAAS = :maas
            WHERE CALISAN_ID = :calisanId
            """;

        updateCommand.Parameters.Add(
            "ad",
            OracleDbType.Varchar2).Value = command.Ad;

        updateCommand.Parameters.Add(
            "soyad",
            OracleDbType.Varchar2).Value = command.Soyad;

        updateCommand.Parameters.Add(
            "maas",
            OracleDbType.Decimal).Value = command.Maas;

        updateCommand.Parameters.Add(
            "calisanId",
            OracleDbType.Int32).Value = command.CalisanId;

        var etkilenen = await updateCommand.ExecuteNonQueryAsync();

        return etkilenen > 0;
    }
}

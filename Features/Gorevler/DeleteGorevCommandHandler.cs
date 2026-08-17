using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Gorevler;

public class DeleteGorevCommandHandler
    : ICommandHandler<DeleteGorevCommand, GorevIslemSonucu>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DeleteGorevCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GorevIslemSonucu> Handle(DeleteGorevCommand command)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var komut = connection.CreateCommand();

        komut.BindByName = true;

        komut.CommandText = """
            DELETE FROM GOREVLER
            WHERE GOREV_ID = :gorevId
            """;

        komut.Parameters.Add("gorevId", OracleDbType.Int32).Value =
            command.GorevId;

        var etkilenenSatir = await komut.ExecuteNonQueryAsync();

        if (etkilenenSatir == 0)
        {
            return new GorevIslemSonucu
            {
                Success = false,
                BulunamadiMi = true,
                Message = "Silinecek görev bulunamadı."
            };
        }

        return new GorevIslemSonucu
        {
            Success = true,
            GorevId = command.GorevId,
            Message = "Görev silindi."
        };
    }
}

using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Calisanlar;

public class DeleteCalisanCommandHandler
    : ICommandHandler<DeleteCalisanCommand, CalisanIslemSonucu>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DeleteCalisanCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CalisanIslemSonucu> Handle(DeleteCalisanCommand command)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var komut = connection.CreateCommand();

        komut.BindByName = true;

        komut.CommandText = """
            DELETE FROM CALISANLAR
            WHERE CALISAN_ID = :calisanId
            """;

        komut.Parameters.Add("calisanId", OracleDbType.Int32).Value =
            command.CalisanId;

        int etkilenenSatir;

        try
        {
            etkilenenSatir = await komut.ExecuteNonQueryAsync();
        }
        catch (OracleException hata) when (hata.Number == 2292)
        {
            return new CalisanIslemSonucu
            {
                Success = false,
                Message = "Bu çalışana bağlı kayıtlar olduğu için silinemiyor."
            };
        }

        if (etkilenenSatir == 0)
        {
            return new CalisanIslemSonucu
            {
                Success = false,
                BulunamadiMi = true,
                Message = "Silinecek çalışan bulunamadı."
            };
        }

        return new CalisanIslemSonucu
        {
            Success = true,
            CalisanId = command.CalisanId,
            Message = "Çalışan silindi."
        };
    }
}

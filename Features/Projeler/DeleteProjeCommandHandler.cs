using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Projeler;

public class DeleteProjeCommandHandler
    : ICommandHandler<DeleteProjeCommand, ProjeIslemSonucu>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DeleteProjeCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ProjeIslemSonucu> Handle(DeleteProjeCommand command)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var komut = connection.CreateCommand();

        komut.BindByName = true;

        komut.CommandText = """
            DELETE FROM PROJELER
            WHERE PROJE_ID = :projeId
            """;

        komut.Parameters.Add("projeId", OracleDbType.Int32).Value =
            command.ProjeId;

        int etkilenenSatir;

        try
        {
            etkilenenSatir = await komut.ExecuteNonQueryAsync();
        }
        catch (OracleException hata) when (hata.Number == 2292)
        {
            return new ProjeIslemSonucu
            {
                Success = false,
                Message = "Bu projeye bağlı görevler olduğu için silinemiyor."
            };
        }

        if (etkilenenSatir == 0)
        {
            return new ProjeIslemSonucu
            {
                Success = false,
                BulunamadiMi = true,
                Message = "Silinecek proje bulunamadı."
            };
        }

        return new ProjeIslemSonucu
        {
            Success = true,
            ProjeId = command.ProjeId,
            Message = "Proje silindi."
        };
    }
}

using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Departmanlar;

public class UpdateDepartmanCommandHandler
    : ICommandHandler<UpdateDepartmanCommand, DepartmanIslemSonucu>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UpdateDepartmanCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DepartmanIslemSonucu> Handle(UpdateDepartmanCommand command)
    {
        var dogrulamaHatasi = DepartmanDogrulama.Kontrol(command.DepartmanAdi);

        if (dogrulamaHatasi != null)
        {
            return new DepartmanIslemSonucu
            {
                Success = false,
                Message = dogrulamaHatasi
            };
        }

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var komut = connection.CreateCommand();

        komut.BindByName = true;

        komut.CommandText = """
            UPDATE DEPARTMANLAR
            SET DEPARTMAN_ADI = :departmanAdi
            WHERE DEPARTMAN_ID = :departmanId
            """;

        komut.Parameters.Add("departmanAdi", OracleDbType.Varchar2).Value =
            command.DepartmanAdi.Trim();

        komut.Parameters.Add("departmanId", OracleDbType.Int32).Value =
            command.DepartmanId;

        int etkilenenSatir;

        try
        {
            etkilenenSatir = await komut.ExecuteNonQueryAsync();
        }
        catch (OracleException hata) when (hata.Number == 1)
        {
            return new DepartmanIslemSonucu
            {
                Success = false,
                Message = "Bu departman adı zaten kayıtlı."
            };
        }

        if (etkilenenSatir == 0)
        {
            return new DepartmanIslemSonucu
            {
                Success = false,
                BulunamadiMi = true,
                Message = "Güncellenecek departman bulunamadı."
            };
        }

        return new DepartmanIslemSonucu
        {
            Success = true,
            DepartmanId = command.DepartmanId,
            Message = "Departman güncellendi."
        };
    }
}

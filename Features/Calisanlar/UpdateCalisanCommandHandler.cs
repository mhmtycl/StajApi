using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Calisanlar;

public class UpdateCalisanCommandHandler
    : ICommandHandler<UpdateCalisanCommand, CalisanIslemSonucu>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UpdateCalisanCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CalisanIslemSonucu> Handle(UpdateCalisanCommand command)
    {
        var dogrulamaHatasi = CalisanDogrulama.Kontrol(
            command.Ad,
            command.Soyad,
            command.Email,
            command.Maas);

        if (dogrulamaHatasi != null)
        {
            return new CalisanIslemSonucu
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
            UPDATE CALISANLAR
            SET AD = :ad,
                SOYAD = :soyad,
                EMAIL = :email,
                MAAS = :maas,
                DEPARTMAN_ID = :departmanId
            WHERE CALISAN_ID = :calisanId
            """;

        komut.Parameters.Add("ad", OracleDbType.Varchar2).Value =
            command.Ad.Trim();

        komut.Parameters.Add("soyad", OracleDbType.Varchar2).Value =
            command.Soyad.Trim();

        komut.Parameters.Add("email", OracleDbType.Varchar2).Value =
            string.IsNullOrWhiteSpace(command.Email)
                ? DBNull.Value
                : (object)command.Email.Trim();

        komut.Parameters.Add("maas", OracleDbType.Decimal).Value =
            (object?)command.Maas ?? DBNull.Value;

        komut.Parameters.Add("departmanId", OracleDbType.Int32).Value =
            (object?)command.DepartmanId ?? DBNull.Value;

        komut.Parameters.Add("calisanId", OracleDbType.Int32).Value =
            command.CalisanId;

        int etkilenenSatir;

        try
        {
            etkilenenSatir = await komut.ExecuteNonQueryAsync();
        }
        catch (OracleException hata) when (hata.Number == 1)
        {
            return new CalisanIslemSonucu
            {
                Success = false,
                Message = "Bu e-posta adresi başka bir çalışana ait."
            };
        }
        catch (OracleException hata) when (hata.Number == 2291)
        {
            return new CalisanIslemSonucu
            {
                Success = false,
                Message = "Seçilen departman bulunamadı."
            };
        }

        if (etkilenenSatir == 0)
        {
            return new CalisanIslemSonucu
            {
                Success = false,
                BulunamadiMi = true,
                Message = "Güncellenecek çalışan bulunamadı."
            };
        }

        return new CalisanIslemSonucu
        {
            Success = true,
            CalisanId = command.CalisanId,
            Message = "Çalışan güncellendi."
        };
    }
}

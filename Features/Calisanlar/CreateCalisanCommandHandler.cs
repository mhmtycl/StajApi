using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;
using System.Data;

namespace StajApi.Features.Calisanlar;

public class CreateCalisanCommandHandler
    : ICommandHandler<CreateCalisanCommand, CalisanIslemSonucu>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CreateCalisanCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CalisanIslemSonucu> Handle(CreateCalisanCommand command)
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
            INSERT INTO CALISANLAR
                (AD, SOYAD, EMAIL, MAAS, DEPARTMAN_ID)
            VALUES
                (:ad, :soyad, :email, :maas, :departmanId)
            RETURNING CALISAN_ID INTO :yeniId
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

        var yeniId = komut.Parameters.Add("yeniId", OracleDbType.Int32);
        yeniId.Direction = ParameterDirection.Output;

        try
        {
            await komut.ExecuteNonQueryAsync();
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

        return new CalisanIslemSonucu
        {
            Success = true,
            CalisanId = Convert.ToInt32(yeniId.Value.ToString()),
            Message = "Çalışan eklendi."
        };
    }
}

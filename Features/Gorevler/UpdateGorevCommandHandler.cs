using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Gorevler;

public class UpdateGorevCommandHandler
    : ICommandHandler<UpdateGorevCommand, GorevIslemSonucu>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UpdateGorevCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GorevIslemSonucu> Handle(UpdateGorevCommand command)
    {
        var dogrulamaHatasi = GorevDogrulama.Kontrol(
            command.GorevAdi,
            command.CalisanId,
            command.ProjeId,
            command.Durum);

        if (dogrulamaHatasi != null)
        {
            return new GorevIslemSonucu
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
            UPDATE GOREVLER
            SET CALISAN_ID = :calisanId,
                PROJE_ID = :projeId,
                GOREV_ADI = :gorevAdi,
                DURUM = :durum,
                TESLIM_TARIHI = :teslimTarihi
            WHERE GOREV_ID = :gorevId
            """;

        komut.Parameters.Add("calisanId", OracleDbType.Int32).Value =
            command.CalisanId;

        komut.Parameters.Add("projeId", OracleDbType.Int32).Value =
            command.ProjeId;

        komut.Parameters.Add("gorevAdi", OracleDbType.Varchar2).Value =
            command.GorevAdi!.Trim();

        komut.Parameters.Add("durum", OracleDbType.Varchar2).Value =
            string.IsNullOrWhiteSpace(command.Durum)
                ? DBNull.Value
                : (object)command.Durum.Trim();

        komut.Parameters.Add("teslimTarihi", OracleDbType.Date).Value =
            (object?)command.TeslimTarihi ?? DBNull.Value;

        komut.Parameters.Add("gorevId", OracleDbType.Int32).Value =
            command.GorevId;

        int etkilenenSatir;

        try
        {
            etkilenenSatir = await komut.ExecuteNonQueryAsync();
        }
        catch (OracleException hata) when (hata.Number == 2291)
        {
            return new GorevIslemSonucu
            {
                Success = false,
                Message = "Seçilen çalışan veya proje bulunamadı."
            };
        }

        if (etkilenenSatir == 0)
        {
            return new GorevIslemSonucu
            {
                Success = false,
                BulunamadiMi = true,
                Message = "Güncellenecek görev bulunamadı."
            };
        }

        return new GorevIslemSonucu
        {
            Success = true,
            GorevId = command.GorevId,
            Message = "Görev güncellendi."
        };
    }
}

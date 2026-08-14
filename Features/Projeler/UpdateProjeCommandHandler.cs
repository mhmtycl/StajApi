using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;

namespace StajApi.Features.Projeler;

public class UpdateProjeCommandHandler
    : ICommandHandler<UpdateProjeCommand, ProjeIslemSonucu>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UpdateProjeCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ProjeIslemSonucu> Handle(UpdateProjeCommand command)
    {
        var dogrulamaHatasi = ProjeDogrulama.Kontrol(
            command.ProjeAdi,
            command.BaslangicTarihi,
            command.BitisTarihi,
            command.Butce);

        if (dogrulamaHatasi != null)
        {
            return new ProjeIslemSonucu
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
            UPDATE PROJELER
            SET PROJE_ADI = :projeAdi,
                BASLANGIC_TARIHI = :baslangicTarihi,
                BITIS_TARIHI = :bitisTarihi,
                BUTCE = :butce
            WHERE PROJE_ID = :projeId
            """;

        komut.Parameters.Add("projeAdi", OracleDbType.Varchar2).Value =
            command.ProjeAdi.Trim();

        komut.Parameters.Add("baslangicTarihi", OracleDbType.Date).Value =
            (object?)command.BaslangicTarihi ?? DBNull.Value;

        komut.Parameters.Add("bitisTarihi", OracleDbType.Date).Value =
            (object?)command.BitisTarihi ?? DBNull.Value;

        komut.Parameters.Add("butce", OracleDbType.Decimal).Value =
            (object?)command.Butce ?? DBNull.Value;

        komut.Parameters.Add("projeId", OracleDbType.Int32).Value =
            command.ProjeId;

        var etkilenenSatir = await komut.ExecuteNonQueryAsync();

        if (etkilenenSatir == 0)
        {
            return new ProjeIslemSonucu
            {
                Success = false,
                BulunamadiMi = true,
                Message = "Güncellenecek proje bulunamadı."
            };
        }

        return new ProjeIslemSonucu
        {
            Success = true,
            ProjeId = command.ProjeId,
            Message = "Proje güncellendi."
        };
    }
}

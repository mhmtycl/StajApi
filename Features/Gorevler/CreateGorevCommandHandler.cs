using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;
using System.Data;

namespace StajApi.Features.Gorevler;

public class CreateGorevCommandHandler
    : ICommandHandler<CreateGorevCommand, GorevIslemSonucu>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CreateGorevCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GorevIslemSonucu> Handle(CreateGorevCommand command)
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
            INSERT INTO GOREVLER
                (CALISAN_ID, PROJE_ID, GOREV_ADI, DURUM, TESLIM_TARIHI)
            VALUES
                (:calisanId, :projeId, :gorevAdi, :durum, :teslimTarihi)
            RETURNING GOREV_ID INTO :yeniId
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

        var yeniId = komut.Parameters.Add("yeniId", OracleDbType.Int32);
        yeniId.Direction = ParameterDirection.Output;

        try
        {
            await komut.ExecuteNonQueryAsync();
        }
        catch (OracleException hata) when (hata.Number == 2291)
        {
            return new GorevIslemSonucu
            {
                Success = false,
                Message = "Seçilen çalışan veya proje bulunamadı."
            };
        }

        return new GorevIslemSonucu
        {
            Success = true,
            GorevId = Convert.ToInt32(yeniId.Value.ToString()),
            Message = "Görev eklendi."
        };
    }
}

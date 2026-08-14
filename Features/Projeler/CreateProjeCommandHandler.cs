using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;
using System.Data;

namespace StajApi.Features.Projeler;

public class CreateProjeCommandHandler
    : ICommandHandler<CreateProjeCommand, ProjeIslemSonucu>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CreateProjeCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ProjeIslemSonucu> Handle(CreateProjeCommand command)
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
            INSERT INTO PROJELER
                (PROJE_ADI, BASLANGIC_TARIHI, BITIS_TARIHI, BUTCE)
            VALUES
                (:projeAdi, :baslangicTarihi, :bitisTarihi, :butce)
            RETURNING PROJE_ID INTO :yeniId
            """;

        komut.Parameters.Add("projeAdi", OracleDbType.Varchar2).Value =
            command.ProjeAdi.Trim();

        komut.Parameters.Add("baslangicTarihi", OracleDbType.Date).Value =
            (object?)command.BaslangicTarihi ?? DBNull.Value;

        komut.Parameters.Add("bitisTarihi", OracleDbType.Date).Value =
            (object?)command.BitisTarihi ?? DBNull.Value;

        komut.Parameters.Add("butce", OracleDbType.Decimal).Value =
            (object?)command.Butce ?? DBNull.Value;

        var yeniId = komut.Parameters.Add("yeniId", OracleDbType.Int32);
        yeniId.Direction = ParameterDirection.Output;

        await komut.ExecuteNonQueryAsync();

        return new ProjeIslemSonucu
        {
            Success = true,
            ProjeId = Convert.ToInt32(yeniId.Value.ToString()),
            Message = "Proje eklendi."
        };
    }
}

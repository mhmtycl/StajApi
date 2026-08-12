using Oracle.ManagedDataAccess.Client;
using StajApi.CQRS;
using StajApi.Data;
using System.Data;

namespace StajApi.Features.Departmanlar;

public class CreateDepartmanCommandHandler
    : ICommandHandler<CreateDepartmanCommand, DepartmanIslemSonucu>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CreateDepartmanCommandHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DepartmanIslemSonucu> Handle(CreateDepartmanCommand command)
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
            INSERT INTO DEPARTMANLAR
                (DEPARTMAN_ADI)
            VALUES
                (:departmanAdi)
            RETURNING DEPARTMAN_ID INTO :yeniId
            """;

        komut.Parameters.Add("departmanAdi", OracleDbType.Varchar2).Value =
            command.DepartmanAdi.Trim();

        var yeniId = komut.Parameters.Add("yeniId", OracleDbType.Int32);
        yeniId.Direction = ParameterDirection.Output;

        try
        {
            await komut.ExecuteNonQueryAsync();
        }
        catch (OracleException hata) when (hata.Number == 1)
        {
            return new DepartmanIslemSonucu
            {
                Success = false,
                Message = "Bu departman adı zaten kayıtlı."
            };
        }

        return new DepartmanIslemSonucu
        {
            Success = true,
            DepartmanId = Convert.ToInt32(yeniId.Value.ToString()),
            Message = "Departman eklendi."
        };
    }
}

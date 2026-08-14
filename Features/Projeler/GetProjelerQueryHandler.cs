using StajApi.CQRS;
using StajApi.Data;
using StajApi.Models;

namespace StajApi.Features.Projeler;

public class GetProjelerQueryHandler
    : IQueryHandler<GetProjelerQuery, List<Proje>>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetProjelerQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Proje>> Handle(GetProjelerQuery query)
    {
        var liste = new List<Proje>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT
                PROJE_ID,
                PROJE_ADI,
                BASLANGIC_TARIHI,
                BITIS_TARIHI,
                BUTCE
            FROM PROJELER
            ORDER BY PROJE_ID";

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            liste.Add(new Proje
            {
                ProjeId = Convert.ToInt32(reader["PROJE_ID"]),
                ProjeAdi = reader["PROJE_ADI"].ToString()!,
                BaslangicTarihi = reader["BASLANGIC_TARIHI"] is DBNull
                    ? null
                    : Convert.ToDateTime(reader["BASLANGIC_TARIHI"]),
                BitisTarihi = reader["BITIS_TARIHI"] is DBNull
                    ? null
                    : Convert.ToDateTime(reader["BITIS_TARIHI"]),
                Butce = reader["BUTCE"] is DBNull
                    ? null
                    : Convert.ToDecimal(reader["BUTCE"])
            });
        }

        return liste;
    }
}

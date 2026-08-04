using StajApi.CQRS;
using StajApi.Data;
using StajApi.Models;

namespace StajApi.Features.Calisanlar;

public class GetCalisanlarQueryHandler
    : IQueryHandler<GetCalisanlarQuery, List<Calisan>>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetCalisanlarQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Calisan>> Handle(GetCalisanlarQuery query)
    {
        var liste = new List<Calisan>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT
                CALISAN_ID,
                AD,
                SOYAD,
                MAAS
            FROM CALISANLAR
            ORDER BY CALISAN_ID";

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            liste.Add(new Calisan
            {
                CalisanId = Convert.ToInt32(reader["CALISAN_ID"]),
                Ad = reader["AD"].ToString()!,
                Soyad = reader["SOYAD"].ToString()!,
                Maas = Convert.ToDecimal(reader["MAAS"])
            });
        }

        return liste;
    }
}

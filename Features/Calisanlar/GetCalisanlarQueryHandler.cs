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
                c.CALISAN_ID,
                c.AD,
                c.SOYAD,
                c.EMAIL,
                c.MAAS,
                c.DEPARTMAN_ID,
                d.DEPARTMAN_ADI
            FROM CALISANLAR c
            LEFT JOIN DEPARTMANLAR d
                ON d.DEPARTMAN_ID = c.DEPARTMAN_ID
            ORDER BY c.CALISAN_ID";

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            liste.Add(new Calisan
            {
                CalisanId = Convert.ToInt32(reader["CALISAN_ID"]),
                Ad = reader["AD"].ToString()!,
                Soyad = reader["SOYAD"].ToString()!,
                Email = reader["EMAIL"] as string,
                Maas = reader["MAAS"] is DBNull
                    ? null
                    : Convert.ToDecimal(reader["MAAS"]),
                DepartmanId = reader["DEPARTMAN_ID"] is DBNull
                    ? null
                    : Convert.ToInt32(reader["DEPARTMAN_ID"]),
                DepartmanAdi = reader["DEPARTMAN_ADI"] as string
            });
        }

        return liste;
    }
}

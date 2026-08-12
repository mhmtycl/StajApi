using StajApi.CQRS;
using StajApi.Data;
using StajApi.Models;

namespace StajApi.Features.Yoneticiler;

public class GetYoneticilerQueryHandler
    : IQueryHandler<GetYoneticilerQuery, List<Yonetici>>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetYoneticilerQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Yonetici>> Handle(GetYoneticilerQuery query)
    {
        var liste = new List<Yonetici>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT
                YONETICI_ID,
                AD,
                SOYAD,
                DEPARTMAN
            FROM YONETICILER
            ORDER BY YONETICI_ID";

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            liste.Add(new Yonetici
            {
                YoneticiId = Convert.ToInt32(reader["YONETICI_ID"]),
                Ad = reader["AD"].ToString()!,
                Soyad = reader["SOYAD"].ToString()!,
                Departman = reader["DEPARTMAN"].ToString()!
            });
        }

        return liste;
    }
}

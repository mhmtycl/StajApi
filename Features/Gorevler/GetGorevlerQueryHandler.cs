using StajApi.CQRS;
using StajApi.Data;
using StajApi.Models;

namespace StajApi.Features.Gorevler;

public class GetGorevlerQueryHandler
    : IQueryHandler<GetGorevlerQuery, List<Gorev>>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetGorevlerQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Gorev>> Handle(GetGorevlerQuery query)
    {
        var liste = new List<Gorev>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT
                g.GOREV_ID,
                g.CALISAN_ID,
                c.AD || ' ' || c.SOYAD AS CALISAN_ADI,
                g.PROJE_ID,
                p.PROJE_ADI,
                g.GOREV_ADI,
                g.DURUM,
                g.TESLIM_TARIHI
            FROM GOREVLER g
            JOIN CALISANLAR c
                ON c.CALISAN_ID = g.CALISAN_ID
            JOIN PROJELER p
                ON p.PROJE_ID = g.PROJE_ID
            ORDER BY g.GOREV_ID";

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            liste.Add(new Gorev
            {
                GorevId = Convert.ToInt32(reader["GOREV_ID"]),
                CalisanId = Convert.ToInt32(reader["CALISAN_ID"]),
                CalisanAdi = reader["CALISAN_ADI"].ToString()!,
                ProjeId = Convert.ToInt32(reader["PROJE_ID"]),
                ProjeAdi = reader["PROJE_ADI"].ToString()!,
                GorevAdi = reader["GOREV_ADI"] as string,
                Durum = reader["DURUM"] as string,
                TeslimTarihi = reader["TESLIM_TARIHI"] is DBNull
                    ? null
                    : Convert.ToDateTime(reader["TESLIM_TARIHI"])
            });
        }

        return liste;
    }
}

using StajApi.CQRS;
using StajApi.Data;
using StajApi.Models;

namespace StajApi.Features.Departmanlar;

public class GetDepartmanlarQueryHandler
    : IQueryHandler<GetDepartmanlarQuery, List<Departman>>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetDepartmanlarQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Departman>> Handle(GetDepartmanlarQuery query)
    {
        var liste = new List<Departman>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT
                DEPARTMAN_ID,
                DEPARTMAN_ADI
            FROM DEPARTMANLAR
            ORDER BY DEPARTMAN_ADI";

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            liste.Add(new Departman
            {
                DepartmanId = Convert.ToInt32(reader["DEPARTMAN_ID"]),
                DepartmanAdi = reader["DEPARTMAN_ADI"].ToString()!
            });
        }

        return liste;
    }
}

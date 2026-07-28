using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using StajApi.Models;

namespace StajApi.Controllers;

[ApiController]
[Authorize]
[Route("api/calisanlar")]
public class CalisanlarController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public CalisanlarController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> GetCalisanlar()
    {
        var liste = new List<Calisan>();

        var connectionString = _configuration.GetConnectionString("OracleDb");

        await using var connection = new OracleConnection(connectionString);
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

        return Ok(liste);
    }
}
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace StajApi.Controllers;

[ApiController]
[Route("api/database")]
public class DatabaseController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public DatabaseController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("OracleDb");

            await using var connection = new OracleConnection(connectionString);
            await connection.OpenAsync();

            return Ok(new
            {
                success = true,
                message = "Oracle bağlantısı başarılı!"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = ex.Message
            });
        }
    }
}
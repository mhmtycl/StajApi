using Microsoft.AspNetCore.Mvc;
using StajApi.Data;

namespace StajApi.Controllers;

[ApiController]
[Route("api/database")]
public class DatabaseController : ControllerBase
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DatabaseController(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        try
        {
            await using var connection = _connectionFactory.CreateConnection();
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

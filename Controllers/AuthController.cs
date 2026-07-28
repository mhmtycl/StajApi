using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Oracle.ManagedDataAccess.Client;
using StajApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StajApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // Geçici test kullanıcısı
        if (request.Username != "admin" || request.Password != "123456")
        {
            return Unauthorized(new
            {
                success = false,
                message = "Kullanıcı adı veya şifre hatalı."
            });
        }

        var accessToken = CreateToken(request.Username);
        var refreshToken = GenerateRefreshToken();

        SaveRefreshToken(request.Username, refreshToken);

        return Ok(new
        {
            success = true,
            accessToken,
            refreshToken,
            expiresInMinutes = 60
        });
    }

    private string CreateToken(string username)
    {
        var key = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var expireMinutes =
            int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60");

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "JWT anahtarı appsettings.json içinde bulunamadı.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, username),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, "Admin"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var securityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));
            
    }

    private void SaveRefreshToken(
        string username,
        string refreshToken)
    {
        
        var connectionString =
            _configuration.GetConnectionString("OracleDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "OracleDb bağlantı bilgisi bulunamadı.");
        }

        using var connection =
            new OracleConnection(connectionString);

        connection.Open();

        using var command = connection.CreateCommand();

        command.BindByName = true;

        command.CommandText = """
            INSERT INTO REFRESH_TOKENS
                (USERNAME, TOKEN, EXPIRES_AT, IS_REVOKED)
            VALUES
                (:username, :token, :expiresAt, 0)
            """;

        command.Parameters.Add(
            "username",
            OracleDbType.Varchar2).Value = username;

        command.Parameters.Add(
            "token",
            OracleDbType.Varchar2).Value = refreshToken;

        command.Parameters.Add(
            "expiresAt",
            OracleDbType.TimeStamp).Value =
            DateTime.UtcNow.AddDays(7);

            command.ExecuteNonQuery();
        }
            [AllowAnonymous]
    [HttpPost("refresh")]
    public IActionResult Refresh(RefreshTokenRequest request)
    {
    var connectionString = _configuration.GetConnectionString("OracleDb");

    using var connection = new OracleConnection(connectionString);
    connection.Open();

    using var command = connection.CreateCommand();

    command.BindByName = true;

    command.CommandText = @"
        SELECT USERNAME
        FROM REFRESH_TOKENS
        WHERE TOKEN = :token
          AND IS_REVOKED = 0
          AND EXPIRES_AT > SYSTIMESTAMP";

    command.Parameters.Add("token", OracleDbType.Varchar2).Value =
        request.RefreshToken;

    var username = command.ExecuteScalar() as string;

        if (username == null)
        {
            return Unauthorized(new
            {
                message = "Geçersiz veya süresi dolmuş Refresh Token."
            });
        }

        var newAccessToken = CreateToken(username);
        var newRefreshToken = GenerateRefreshToken();

        RevokeRefreshToken(request.RefreshToken);

        SaveRefreshToken(username, newRefreshToken);

             return Ok(new
        {
            success = true,
            accessToken = newAccessToken,
            refreshToken = newRefreshToken
        });
    }

    private void RevokeRefreshToken(string refreshToken)
    {
        var connectionString =
            _configuration.GetConnectionString("OracleDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "OracleDb bağlantı bilgisi bulunamadı.");
        }

        using var connection =
            new OracleConnection(connectionString);

        connection.Open();

        using var command = connection.CreateCommand();

        command.BindByName = true;

        command.CommandText = """
            UPDATE REFRESH_TOKENS
            SET IS_REVOKED = 1
            WHERE TOKEN = :token
            """;

        command.Parameters.Add(
            "token",
            OracleDbType.Varchar2).Value = refreshToken;

        command.ExecuteNonQuery();
    }
}
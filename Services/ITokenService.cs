namespace StajApi.Services;

public interface ITokenService
{
    string CreateAccessToken(string username);
    string GenerateRefreshToken();
}

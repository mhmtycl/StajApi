namespace StajApi.Repositories;

public interface IRefreshTokenRepository
{
    Task SaveAsync(string username, string refreshToken);
    Task<string?> GetActiveUsernameAsync(string refreshToken);
    Task RevokeAsync(string refreshToken);
}

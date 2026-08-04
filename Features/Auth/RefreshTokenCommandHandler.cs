using StajApi.CQRS;
using StajApi.Repositories;
using StajApi.Services;

namespace StajApi.Features.Auth;

public class RefreshTokenCommandHandler
    : ICommandHandler<RefreshTokenCommand, AuthResult>
{
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RefreshTokenCommandHandler(
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResult> Handle(RefreshTokenCommand command)
    {
        var username =
            await _refreshTokenRepository.GetActiveUsernameAsync(
                command.RefreshToken);

        if (username == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Geçersiz veya süresi dolmuş Refresh Token."
            };
        }

        var newAccessToken = _tokenService.CreateAccessToken(username);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        await _refreshTokenRepository.RevokeAsync(command.RefreshToken);
        await _refreshTokenRepository.SaveAsync(username, newRefreshToken);

        return new AuthResult
        {
            Success = true,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}

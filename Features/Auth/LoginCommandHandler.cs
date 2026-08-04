using StajApi.CQRS;
using StajApi.Repositories;
using StajApi.Services;

namespace StajApi.Features.Auth;

public class LoginCommandHandler
    : ICommandHandler<LoginCommand, AuthResult>
{
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginCommandHandler(
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResult> Handle(LoginCommand command)
    {
        // Geçici test kullanıcısı
        if (command.Username != "admin" || command.Password != "123456")
        {
            return new AuthResult
            {
                Success = false,
                Message = "Kullanıcı adı veya şifre hatalı."
            };
        }

        var accessToken = _tokenService.CreateAccessToken(command.Username);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _refreshTokenRepository.SaveAsync(command.Username, refreshToken);

        return new AuthResult
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}

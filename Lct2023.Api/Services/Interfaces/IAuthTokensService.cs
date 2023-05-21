namespace Lct2023.Api.Services.Interfaces;

public interface IAuthTokensService
{
    string CreateAccessToken(int userId);

    (string Token, DateTimeOffset ExpiresAt) CreateRefreshToken();

    bool ValidateAccessToken(string accessToken);
}

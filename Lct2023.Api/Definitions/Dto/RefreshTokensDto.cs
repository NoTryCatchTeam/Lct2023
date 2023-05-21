namespace Lct2023.Api.Definitions.Dto;

public class RefreshTokensDto
{
    public int UserId { get; set; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}

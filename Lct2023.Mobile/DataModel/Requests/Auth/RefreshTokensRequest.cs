namespace DataModel.Requests.Auth;

public class RefreshTokensRequest
{
    public int UserId { get; set; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}

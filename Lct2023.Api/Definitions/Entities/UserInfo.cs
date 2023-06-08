using Lct2023.Api.Definitions.Identity;

public class UserInfo
{
    public int Id { get; set; }

    public int ExtendedIdentityUserId { get; set; }
    public ExtendedIdentityUser User { get; set; } = null!;
    public int Rating { get; set; }
}
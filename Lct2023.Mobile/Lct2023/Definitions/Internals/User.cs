namespace Lct2023.Definitions.Internals;

public class User
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string PhotoUrl { get; set; }

    public bool IsProfTestFinished { get; set; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public int Rating { get; set; }
}

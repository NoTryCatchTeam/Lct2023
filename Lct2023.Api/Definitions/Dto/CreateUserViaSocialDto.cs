namespace Lct2023.Api.Definitions.Dto;

public class CreateUserViaSocialDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string PhotoUrl { get; set; }

    public string LoginProvider { get; set; }

    public string LoginProviderKey { get; set; }
}

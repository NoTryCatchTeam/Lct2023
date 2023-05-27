namespace Lct2023.Api.Definitions.Dto;

public class CreateUserDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public DateTime? BirthDate { get; set; }

    public string Photo { get; set; }
}

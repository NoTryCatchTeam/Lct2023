using System;

namespace Lct2023.Definitions.Dtos;

public class CreateUserDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string Photo { get; set; }

    public DateTimeOffset? BirthDate { get; set; }
}

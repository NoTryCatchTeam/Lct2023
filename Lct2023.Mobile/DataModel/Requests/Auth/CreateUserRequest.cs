using System;

namespace DataModel.Requests.Auth;

public class CreateUserRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public DateTime? BirthDate { get; set; }

    public string Photo { get; set; }
}

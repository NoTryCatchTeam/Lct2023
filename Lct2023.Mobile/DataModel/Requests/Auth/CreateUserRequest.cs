namespace DataModel.Requests.Auth;

public class CreateUserRequest
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public override string ToString() =>
        $"{FirstName} {LastName}, {Email}";
}

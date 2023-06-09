using System;

namespace DataModel.Responses.Users;

public class UserItemResponse
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string PhotoUrl { get; set; }

    public DateTime BirthDate {get;set;}
}

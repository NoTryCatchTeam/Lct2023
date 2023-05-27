using System.ComponentModel.DataAnnotations;
using Lct2023.Api.Definitions.Entities;
using Microsoft.AspNetCore.Identity;

namespace Lct2023.Api.Definitions.Identity;

public class ExtendedIdentityUser : IdentityUser<int>
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public DateTime? BirthDate { get; set; }

    public string PhotoUrl { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public IEnumerable<UserRefreshToken> RefreshTokens { get; set; }
}

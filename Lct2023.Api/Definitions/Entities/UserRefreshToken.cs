using System.ComponentModel.DataAnnotations;
using Lct2023.Api.Definitions.Identity;

namespace Lct2023.Api.Definitions.Entities;

public class UserRefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    public ExtendedIdentityUser User { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    public DateTimeOffset ExpiresAt { get; set; }
}

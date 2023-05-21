using System.Security.Claims;

namespace Lct2023.Api.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static int GetId(this ClaimsPrincipal principal) =>
        int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
}

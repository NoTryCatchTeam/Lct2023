using AspNet.Security.OAuth.Vkontakte;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lct2023.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet]
    [Route("sign-in/vk")]
    [AllowAnonymous]
    public IActionResult SignInViaVkAsync() =>
        // TODO Change with below
        // _signInManager.ConfigureExternalAuthenticationProperties(
        //     VkontakteAuthenticationDefaults.AuthenticationScheme,
        //     Url.Action("SignInSuccessful")),
        Challenge(
            new OAuthChallengeProperties { RedirectUri = Url.Action("SignInSuccessful") },
            VkontakteAuthenticationDefaults.AuthenticationScheme);

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("signin-success")]
    public async Task<IActionResult> SignInSuccessfulAsync()
    {
        // TODO For mobile app add redirect
        return Ok();
    }
}

using AspNet.Security.OAuth.Vkontakte;
using AutoMapper;
using DataModel.Requests.Auth;
using DataModel.Responses.Auth;
using DataModel.Responses.Users;
using Lct2023.Api.Definitions.Dto;
using Lct2023.Api.Definitions.Identity;
using Lct2023.Api.Definitions.Types;
using Lct2023.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Lct2023.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly SignInManager<ExtendedIdentityUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IUserService userService,
        SignInManager<ExtendedIdentityUser> signInManager,
        IMapper mapper,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _userService = userService;
        _signInManager = signInManager;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    [Route("sign-up")]
    [ProducesResponseType(typeof(UserItemResponse), 200)]
    [ProducesResponseType(400)]
    [AllowAnonymous]
    public async Task<IActionResult> SignUpAsync([FromBody] CreateUserRequest data)
    {
        try
        {
            return Ok(_mapper.Map<UserItemResponse>(await _authService.SignUpAsync(_mapper.Map<CreateUserDto>(data))));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't sign in user");
        }

        return BadRequest();
    }

    [HttpPost]
    [Route("sign-in/basic")]
    [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
    [ProducesResponseType(400)]
    [AllowAnonymous]
    public async Task<IActionResult> SignInViaBasicAsync([FromBody] SignInBasicRequest data)
    {
        try
        {
            return Ok(_mapper.Map<AuthSuccessResponse>(await _authService.SignInAsync(data.Login, data.Password)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't sign in user");
        }

        return BadRequest();
    }

    [HttpGet]
    [Route("sign-in/vk")]
    [AllowAnonymous]
    public IActionResult SignInViaVkAsync() =>
        Challenge(
            _signInManager.ConfigureExternalAuthenticationProperties(
                VkontakteAuthenticationDefaults.AuthenticationScheme,
                Url.Action("SignInSuccessful")),
            VkontakteAuthenticationDefaults.AuthenticationScheme);

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("signin-success")]
    public async Task<IActionResult> SignInSuccessfulAsync()
    {
        var userData = QueryHelpers.ParseQuery(Request.QueryString.Value);
        var email = userData[CustomClaimTypes.EMAIL].ToString();
        var firstName = userData[CustomClaimTypes.FIRST_NAME].ToString();
        var lastName = userData[CustomClaimTypes.LAST_NAME].ToString();
        userData.TryGetValue(CustomClaimTypes.PHOTO_URL, out var photoUrl);
        var provider = userData[CustomClaimTypes.EXTERNAL_LOGIN_PROVIDER].ToString();
        var providerKey = userData[CustomClaimTypes.EXTERNAL_LOGIN_PROVIDER_KEY].ToString();

        try
        {
            var user = await _userService.GetByEmailAsync(email) ??
                       await _authService.SignUpViaSocialAsync(new CreateUserViaSocialDto
                       {
                           Email = email,
                           FirstName = firstName,
                           LastName = lastName,
                           PhotoUrl = photoUrl,
                           LoginProvider = provider,
                           LoginProviderKey = providerKey,
                       });

            var authResponse = await _authService.SignInViaSocialAsync(user.Username, provider);

            return Redirect($"lct2023://access_token={authResponse.AccessToken}&refresh_token={authResponse.RefreshToken}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't sign up user");
        }

        return Redirect("lct2023://error=auth_error");
    }

    [HttpPost]
    [Route("refresh")]
    [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
    [ProducesResponseType(400)]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokensRequest data)
    {
        try
        {
            return Ok(_mapper.Map<AuthSuccessResponse>(await _authService.RefreshAsync(_mapper.Map<RefreshTokensDto>(data))));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't refresh token");
        }

        return BadRequest();
    }
}

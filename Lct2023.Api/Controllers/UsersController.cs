using AutoMapper;
using DataModel.Responses.Users;
using Lct2023.Api.Helpers;
using Lct2023.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lct2023.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, IMapper mapper, ILogger<UsersController> logger)
    {
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    [Route("self")]
    [ProducesResponseType(typeof(UserItemResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSelfAsync()
    {
        try
        {
            if (await _userService.GetByIdAsync(User.GetId()) is not { } user)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserItemResponse>(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't sign in user");
        }

        return BadRequest();
    }
}

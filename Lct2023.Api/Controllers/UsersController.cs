using AutoMapper;
using DataModel.Responses.Users;
using Lct2023.Api.Helpers;
using Lct2023.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lct2023.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IRatingService _ratingService;
    private readonly IMapper _mapper;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, IMapper mapper, ILogger<UsersController> logger, IRatingService ratingService)
    {
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
        _ratingService = ratingService;
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
            _logger.LogError(ex, "Couldn't get user");
        }

        return BadRequest();
    }

    [HttpPost]
    [Route("rating")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> AddRatingAsync(int increment)
    {
        try
        {
            await _ratingService.AddRatingAsync(User.GetId(), increment);

            return Ok(await _ratingService.GetRatingAsync(User.GetId()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't add rating");
        }

        return BadRequest();
    }

    [HttpGet]
    [Route("rating")]
    [ProducesResponseType(typeof(int), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetRatingAsync()
    {
        try
        {
            return Ok(await _ratingService.GetRatingAsync(User.GetId()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't get rating");
        }

        return BadRequest();
    }
}

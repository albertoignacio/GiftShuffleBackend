using System.Security.Claims;
using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiftShuffle.Api.Controllers;

[ApiController]
[Route("api/shuffle")]
[Authorize]
public class ShuffleController : ControllerBase
{
    private readonly IShuffleService _shuffleService;

    public ShuffleController(IShuffleService shuffleService)
    {
        _shuffleService = shuffleService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<ShuffleResponse>> ExecuteShuffle([FromBody] ShuffleRequest request)
    {
        try
        {
            var response = await _shuffleService.ExecuteShuffleAsync(GetUserId(), request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}

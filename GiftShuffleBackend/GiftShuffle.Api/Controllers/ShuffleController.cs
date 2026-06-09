using System.Security.Claims;
using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiftShuffle.Api.Controllers;

[ApiController]
[Route("api/shuffle")]
[Authorize]
[Tags("Shuffle")]
public class ShuffleController(IShuffleService shuffleService) : ControllerBase
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [EndpointSummary("Ejecuta el sorteo de amigo invisible")]
    [ProducesResponseType<ShuffleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShuffleResponse>> ExecuteShuffle([FromBody] ShuffleRequest request, CancellationToken ct)
    {
        var response = await shuffleService.ExecuteShuffleAsync(GetUserId(), request, ct);
        return Ok(response);
    }
}
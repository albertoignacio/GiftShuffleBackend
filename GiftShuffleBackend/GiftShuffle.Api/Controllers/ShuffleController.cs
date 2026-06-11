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
        string? name = null, lastName = null, email = null;
        if (request.IncludeCurrentUser)
        {
            name = User.FindFirstValue(ClaimTypes.Name);
            lastName = User.FindFirstValue("lastName");
            email = User.FindFirstValue(ClaimTypes.Email);
        }
        var response = await shuffleService.ExecuteShuffleAsync(GetUserId(), request,
            name, lastName, email, ct);
        return Ok(response);
    }

    [HttpDelete("history")]
    [EndpointSummary("Limpia el historial de sorteos anteriores")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ClearHistory(CancellationToken ct)
    {
        await shuffleService.ClearHistoryAsync(GetUserId(), ct);
        return Ok(new { message = "Historial limpiado" });
    }
}

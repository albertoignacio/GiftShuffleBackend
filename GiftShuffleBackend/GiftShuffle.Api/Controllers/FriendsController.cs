using System.Security.Claims;
using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiftShuffle.Api.Controllers;

[ApiController]
[Route("api/friends")]
[Authorize]
[Tags("Friends")]
public class FriendsController(IFriendService friendService) : ControllerBase
{
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("getAll")]
    [EndpointSummary("Obtiene todos los amigos del usuario autenticado")]
    [ProducesResponseType<List<FriendResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<FriendResponse>>> GetAll(CancellationToken ct)
    {
        var friends = await friendService.GetAllAsync(GetUserId(), ct);
        return Ok(friends);
    }

    [HttpGet("{id}")]
    [EndpointSummary("Obtiene un amigo por su ID")]
    [ProducesResponseType<FriendResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FriendResponse>> GetById(Guid id, CancellationToken ct)
    {
        var friend = await friendService.GetByIdAsync(id, GetUserId(), ct);
        if (friend == null) return NotFound();
        return Ok(friend);
    }

    [HttpPost("create")]
    [EndpointSummary("Crea un nuevo amigo")]
    [ProducesResponseType<FriendResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FriendResponse>> Create([FromBody] CreateFriendRequest request, CancellationToken ct)
    {
        var friend = await friendService.CreateAsync(GetUserId(), request, ct);
        return CreatedAtAction(nameof(GetById), new { id = friend.Id }, friend);
    }

    [HttpPut("{id}")]
    [EndpointSummary("Actualiza los datos de un amigo")]
    [ProducesResponseType<FriendResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FriendResponse>> Update(Guid id, [FromBody] UpdateFriendRequest request, CancellationToken ct)
    {
        var friend = await friendService.UpdateAsync(id, GetUserId(), request, ct);
        return Ok(friend);
    }

    [HttpDelete("{id}")]
    [EndpointSummary("Elimina un amigo")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        await friendService.DeleteAsync(id, GetUserId(), ct);
        return NoContent();
    }
}
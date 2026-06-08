using System.Security.Claims;
using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiftShuffle.Api.Controllers;

[ApiController]
[Route("api/friends")]
[Authorize]
public class FriendsController : ControllerBase
{
    private readonly IFriendService _friendService;

    public FriendsController(IFriendService friendService)
    {
        _friendService = friendService;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<FriendResponse>>> GetAll()
    {
        var friends = await _friendService.GetAllAsync(GetUserId());
        return Ok(friends);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FriendResponse>> GetById(Guid id)
    {
        var friend = await _friendService.GetByIdAsync(id, GetUserId());
        if (friend == null) return NotFound();
        return Ok(friend);
    }

    [HttpPost]
    public async Task<ActionResult<FriendResponse>> Create([FromBody] CreateFriendRequest request)
    {
        var friend = await _friendService.CreateAsync(GetUserId(), request);
        return CreatedAtAction(nameof(GetById), new { id = friend.Id }, friend);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<FriendResponse>> Update(Guid id, [FromBody] UpdateFriendRequest request)
    {
        try
        {
            var friend = await _friendService.UpdateAsync(id, GetUserId(), request);
            return Ok(friend);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _friendService.DeleteAsync(id, GetUserId());
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

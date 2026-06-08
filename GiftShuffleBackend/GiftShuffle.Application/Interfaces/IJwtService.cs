using GiftShuffle.Application.DTOs;

namespace GiftShuffle.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(TokenUserInfo user);
}

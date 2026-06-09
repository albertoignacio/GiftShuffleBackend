using GiftShuffle.Application.DTOs;

namespace GiftShuffle.Application.Interfaces;

/// <summary>Generates JWT tokens for authenticated users.</summary>
public interface IJwtService
{
    string GenerateToken(TokenUserInfo user);
}
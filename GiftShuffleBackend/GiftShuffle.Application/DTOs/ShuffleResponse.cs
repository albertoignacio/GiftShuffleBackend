namespace GiftShuffle.Application.DTOs;

public record ShuffleResponse(bool Shuffled, int ParticipantCount, decimal GiftAmount);

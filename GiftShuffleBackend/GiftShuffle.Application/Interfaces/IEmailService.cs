namespace GiftShuffle.Application.Interfaces;

/// <summary>Sends email notifications for shuffle assignments.</summary>
public interface IEmailService
{
    Task SendAssignmentEmailAsync(string toEmail, string toName, string receiverName, decimal giftAmount, CancellationToken ct = default);
}
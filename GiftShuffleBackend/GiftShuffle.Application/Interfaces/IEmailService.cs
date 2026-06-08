namespace GiftShuffle.Application.Interfaces;

public interface IEmailService
{
    Task SendAssignmentEmailAsync(string toEmail, string toName, string receiverName, decimal giftAmount);
}

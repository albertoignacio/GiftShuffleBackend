using GiftShuffle.Application.Interfaces;
using GiftShuffle.Infraestructure.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GiftShuffle.Infraestructure.Services;

public class EmailService(IOptions<SmtpOptions> smtpOptions, ILogger<EmailService> logger) : IEmailService
{
    private readonly SmtpOptions _smtp = smtpOptions.Value;

    public async Task SendAssignmentEmailAsync(string toEmail, string toName, string receiverName, decimal giftAmount, CancellationToken ct = default)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Amigo Invisible", _smtp.Username));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = "Tu amigo invisible ha sido asignado";

        message.Body = new TextPart("plain")
        {
            Text = $"Hola {toName}!\n\n" +
                   $"Te ha tocado regalarle a: {receiverName}\n" +
                   $"Monto sugerido: ${giftAmount:F2}\n\n" +
                   "¡Feliz intercambio!"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtp.Host, _smtp.Port, MailKit.Security.SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(_smtp.Username, _smtp.Password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);

        logger.LogInformation("Email sent to {Email} about {Receiver}", toEmail, receiverName);
    }

    public async Task SendVerificationEmailAsync(string toEmail, string toName, string confirmationLink, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Amigo Invisible", _smtp.Username));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = "Confirma tu email - Amigo Invisible";

        message.Body = new TextPart("plain")
        {
            Text = $"Hola {toName}!\n\n" +
                   $"Por favor confirma tu email haciendo clic en el siguiente enlace:\n\n" +
                   $"{confirmationLink}\n\n" +
                   "¡Gracias!"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtp.Host, _smtp.Port, MailKit.Security.SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(_smtp.Username, _smtp.Password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);

        logger.LogInformation("Verification email sent to {Email}", toEmail);
    }
}

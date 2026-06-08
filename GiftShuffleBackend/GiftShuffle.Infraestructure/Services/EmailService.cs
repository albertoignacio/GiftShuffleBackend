using GiftShuffle.Application.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace GiftShuffle.Infraestructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAssignmentEmailAsync(string toEmail, string toName, string receiverName, decimal giftAmount)
    {
        var message = new MimeMessage();
        var username = _configuration["Smtp:Username"]!;
        var host = _configuration["Smtp:Host"]!;
        var port = int.Parse(_configuration["Smtp:Port"] ?? "587");
        var password = _configuration["Smtp:Password"]!;

        message.From.Add(new MailboxAddress("Gift Shuffle", username));
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
        await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);

        await client.AuthenticateAsync(username, password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}

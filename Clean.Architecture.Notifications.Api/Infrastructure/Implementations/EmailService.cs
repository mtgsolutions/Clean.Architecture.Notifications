using Clean.Architecture.Notifications.Api.Infrastructure.Contracts;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Clean.Architecture.Notifications.Api.Infrastructure.Implementations;

public class EmailService : INotificationService
{
    private readonly ISendGridClient _sendGridClient;
    private readonly string? _from;
    private readonly string? _name;

    public EmailService(ISendGridClient sendGridClient, IConfiguration configuration)
    {
        if(configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        _sendGridClient = sendGridClient;
        _from = configuration.GetSection("SendGrid:From").Value;
        _name = configuration.GetSection("SendGrid:Name").Value;
    }

    public async Task Send(IEmailTemplate template)
    {
        var message = new SendGridMessage
        {
            From = new EmailAddress(_from, _name),
            Subject = template.Subject,
            PlainTextContent = template.Content,
            HtmlContent = template.Content
        };
        message.AddTo(new EmailAddress(template.To));

        // Send email
        await _sendGridClient.SendEmailAsync(message);
    }
}


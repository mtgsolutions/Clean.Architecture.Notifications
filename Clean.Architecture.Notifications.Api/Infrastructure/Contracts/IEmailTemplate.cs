namespace Clean.Architecture.Notifications.Api.Infrastructure.Contracts;

public interface IEmailTemplate
{
    string Subject { get; set; }
    string Content { get; set; }
    string To { get; set; }
}

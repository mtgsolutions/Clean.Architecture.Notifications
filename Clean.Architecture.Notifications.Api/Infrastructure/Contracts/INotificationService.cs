namespace Clean.Architecture.Notifications.Api.Infrastructure.Contracts;

public interface INotificationService
{
    Task Send(IEmailTemplate template);
}

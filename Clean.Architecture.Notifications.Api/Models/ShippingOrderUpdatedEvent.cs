namespace Clean.Architecture.Notifications.Api.Models;

public class ShippingOrderUpdatedEvent
{
    public string TrackingNumber { get; set; }
    public string ContactEmail { get; set; }
    public string Description { get; set; }
}

namespace Infrastructure.Models;

public interface IDeliveryCancelRequest
{
    public int DeliveryId { get; set; }
}

public class DeliveryCancelRequest : IDeliveryCancelRequest
{
    public int DeliveryId { get; set; }
}
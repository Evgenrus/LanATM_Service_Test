namespace Infrastructure.Models;

public interface IDeliveryResponse
{
    public int DeliveryId { get; set; }
    public string? ErrMsg { get; set; }
}

public class DeliveryResponse : IDeliveryResponse
{
    public int DeliveryId { get; set; }
    public string? ErrMsg { get; set; }
}
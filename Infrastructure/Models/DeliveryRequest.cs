namespace Infrastructure.Models;

public interface IDeliveryRequest
{
    public int Id { get; set; }
    
    public int? CourierId { get; set; }
    public string? CourierName { get; set; }
    
    public string CustomerLogin { get; set; }
    public string CustomerName { get; set; }
    
    public string Address { get; set; }
    
    public ItemsToCheckList Items { get; set; }
}

public class DeliveryRequest : IDeliveryRequest
{
    public int Id { get; set; }
    public int? CourierId { get; set; }
    public string? CourierName { get; set; }
    public string CustomerLogin { get; set; }
    public string CustomerName { get; set; }
    public string Address { get; set; }
    public ItemsToCheckList Items { get; set; }
}
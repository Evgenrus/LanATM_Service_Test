namespace Delivery.Models;

public class DeliveryModel
{
    public int Id { get; set; }
    
    public int? CourierId { get; set; }
    public string? CourierName { get; set; }
    
    public int OrderId { get; set; }
    
    public string CustomerLogin { get; set; }
    public string CustomerName { get; set; }
    
    public string Status { get; set; }
    
    public string Address { get; set; }
    
    public List<ItemModel> Items { get; set; }
}
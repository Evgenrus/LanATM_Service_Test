namespace Delivery.Database.Entities;

public class DeliveryItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Article { get; set; }
    public string Descr { get; set; }
    public string Brand { get; set; }
    public string Category { get; set; }
    public int Count { get; set; }
    
    public int? DeliveryId { get; set; }
    public OrderDelivery? Delivery { get; set; }
}
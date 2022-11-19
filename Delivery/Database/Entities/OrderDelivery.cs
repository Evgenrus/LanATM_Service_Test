namespace Delivery.Database.Entities;

internal enum Status : int
{
    Pending,
    Assigned,
    InProcess,
    Delivered
}

public class OrderDelivery
{
    public int Id { get; set; }
    
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    
    public int? CourierId { get; set; }
    public Courier? Courier { get; set; }

    public List<DeliveryItem> Items { get; set; } = new List<DeliveryItem>();
}
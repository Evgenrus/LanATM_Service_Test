namespace Delivery.Database.Entities;

public enum Status : int
{
    Pending,
    Assigned,
    InProcess,
    Delivered,
    Canceled
}

public class OrderDelivery
{
    public int Id { get; set; }

    public Status Status { get; set; } = Status.Pending;
    
    public int OrderId { get; set; }

    public int? CourierId { get; set; }
    public Courier? Courier { get; set; }
    
    public int? AddressId { get; set; }
    public Address Address { get; set; }

    public List<DeliveryItem> Items { get; set; } = new List<DeliveryItem>();
}
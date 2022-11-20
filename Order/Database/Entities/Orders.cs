namespace Order.Database.Entities;

public class Orders
{
    public int Id { get; set; }
    public bool IsFinished { get; set; } = false;
    public bool IsCanceled { get; set; } = false;
    public bool IsOnDelivery { get; set; } = false;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
}
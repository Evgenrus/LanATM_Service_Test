namespace Order.Models;

public class PostOrderModel
{
    public bool IsOnDelivery { get; set; } = false;
    
    public int CartId { get; set; }
    
    public string? Address { get; set; }
    public int CustomerId { get; set; }
}
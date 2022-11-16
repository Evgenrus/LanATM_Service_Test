namespace Order.Database.Entities;

public class Carts
{
    public int Id { get; set; }
    
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    public List<CartItem> Items { get; set; } = new List<CartItem>();
}
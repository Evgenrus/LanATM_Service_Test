namespace Order.Database.Entities;

public class CartItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Article { get; set; }
    public string Descr { get; set; }
    public string Brand { get; set; }
    public string Category { get; set; }
    public int Count { get; set; }

    public int CartsId { get; set; }
    public Carts Carts { get; set; }
}
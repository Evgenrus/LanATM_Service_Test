namespace Order.Models;

public class CustomerModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }

    public int Phone { get; set; }
    public string Mail { get; set; }
    
    public List<OrderModel>? Orders { get; set; } = new List<OrderModel>();

    public List<CartModel>? Carts { get; set; } = new List<CartModel>();
}

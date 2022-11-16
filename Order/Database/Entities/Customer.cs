namespace Order.Database.Entities;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }

    public int Phone { get; set; }
    public string Mail { get; set; }
    
    public List<Orders> Orders { get; set; } = new List<Orders>();

    public List<Carts> Carts { get; set; } = new List<Carts>();

    public List<Address> Addresses { get; set; } = new List<Address>();
}
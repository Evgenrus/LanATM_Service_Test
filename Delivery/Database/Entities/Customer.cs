namespace Delivery.Database.Entities;

public class Customer
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }

    public int Phone { get; set; }
    public string Mail { get; set; }
    
    public List<Address> Addresses { get; set; }
    public List<OrderDelivery> Deliveries { get; set; }
}
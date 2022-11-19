namespace Delivery.Database.Entities;

public class Courier
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Phone { get; set; }
    public string Mail { get; set; }
    public string Photo { get; set; }

    public List<OrderDelivery> Deliveries { get; set; }
}
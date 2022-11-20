using System.Text;

namespace Delivery.Database.Entities;

public class Address
{
    public int Id { get; set; }
    public string Region { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Street { get; set; }
    public string House { get; set; }
    public int? Floor { get; set; }
    public string? Flat { get; set; }
    
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    
    public List<OrderDelivery> Deliveries { get; set; }
    
    public override string ToString()
    {
        var sb = new StringBuilder(50);
        sb.AppendFormat($"{Region}, {City}, {District}, {Street}, {House}");
        if (Floor is not null)
            sb.AppendFormat($", {Floor}");
        if (Flat is not null)
            sb.AppendFormat($", {Flat}");

        return sb.ToString();
    }
}
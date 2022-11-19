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
}
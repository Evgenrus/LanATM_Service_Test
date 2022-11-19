using Order.Database.Entities;

namespace Order.Models;

public class OrderModel
{
    public int Id { get; set; }
    public bool IsFinished { get; set; } = false;
    public bool IsCanceled { get; set; } = false;
    public bool IsOnDelivery { get; set; } = false;
    
    public string? CustomerName { get; set; }

    public List<ItemModel> Items { get; set; } = new List<ItemModel>();
}
namespace Order.Models;

public class CartModel
{
    public int? Id { get; set; }
    public List<ItemModel>? Items { get; set; } = new List<ItemModel>();
}
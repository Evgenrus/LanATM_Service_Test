namespace Infrastructure.Models;

public interface IItemsCheck
{
    public List<ItemCheck> Items { get; set; }
}

public class ItemsCheck : IItemsCheck
{
    public List<ItemCheck> Items { get; set; }
}
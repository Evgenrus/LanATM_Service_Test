namespace Infrastructure.Models;

public interface IItemsToCheckList
{
    public List<ItemToCheck> Items { get; set; }
}

public class ItemsToCheckList : IItemsToCheckList
{
    public List<ItemToCheck> Items { get; set; }
}
using Catalog.Models;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Services;

public interface ICatalogService
{
    public Task<ItemModel> GetItemById(int id);

    public Task<ICollection<ItemModel>> GetItemsByBrandName(string brand);

    public Task<ItemModel> GetItemByName(string name);

    public Task<ICollection<ItemModel>> GetItemsByCategoryName(string category);

    public Task PostNewItem(ItemModel model);
    
    public Task PostNewBrand(BrandModel model);

    public Task PostNewCategory(CategoryModel model);

    public Task Restock(string article, int count);

    public Task<ItemModel> AddItemToCart(ItemModel model);

    public Task<ICollection<ItemModel>> OrderItems(List<ItemModel> items);

    public Task<ItemsCheck> CheckItems(ItemsToCheckList toChecks);
    public Task<ItemCheck> CheckItem(ItemToCheck toCheck);
}
using System.Web.Http.ModelBinding;
using Catalog.Database;
using Catalog.Database.Entities;
using Catalog.Exceptions;
using Catalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Services;

public class CatalogService : ICatalogService
{
    private CatalogDbContext _context;
    
    public CatalogService(CatalogDbContext context)
    {
        _context = context;
    }
    
    public async Task<ItemModel> GetItemById(int id)
    {
        if (id < 1)
        {
            throw new IncorrectIdException($"id must be >= 1, but got {id}");
        }

        var item = await _context.Items.Include(x => x.Brand)
            .Include(x => x.Category)
            .SingleOrDefaultAsync(y => y.Id == id);

        if (item is null)
        {
            throw new ItemNotFoundException($"Couldn't find Item with id {id}");
        }

        return new ItemModel()
        {
            Article = item.Article,
            Brand = item.Brand.Name,
            Category = item.Category.Name,
            Descr = item.Descr,
            Name = item.Name,
            Stock = item.Stock,
            Id = item.Id
        };
    }

    public async Task<ICollection<ItemModel>> GetItemsByBrandName(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new InvalidBrandException("Brand name is null, empty or contains only whitespaces");

        var items = await _context.Items.Include(x => x.Brand)
            .Include(x => x.Category)
            .Where(y => y.Brand.Name == brand)
            .ToListAsync();

        if (!items.Any())
            throw new EmptyResultException("No items");

        var result = new List<ItemModel>();
        foreach (var item in items)
        {
            result.Add(new ItemModel()
            {
                Article = item.Article,
                Brand = item.Brand.Name,
                Category = item.Category.Name,
                Descr = item.Descr,
                Name = item.Name,
                Stock = item.Stock,
                Id = item.Id
            });
        }

        return result;
    }

    public async Task<ItemModel> GetItemByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidItemException("name is null, empty or contains only whitespaces");

        var item = await _context.Items.Include(x => x.Brand)
            .Include(x => x.Category)
            .SingleOrDefaultAsync(y => y.Name == name);

        if (item is null)
            throw new ItemNotFoundException($"No items with name {name}");
        
        return new ItemModel()
        {
            Article = item.Article,
            Brand = item.Brand.Name,
            Category = item.Category.Name,
            Descr = item.Descr,
            Name = item.Name,
            Stock = item.Stock,
            Id = item.Id
        };
    }

    public async Task<ICollection<ItemModel>> GetItemsByCategoryName(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new InvalidCategoryException("Brand name is null, empty or contains only whitespaces");

        var items = await _context.Items.Include(x => x.Brand)
            .Include(x => x.Category)
            .Where(y => y.Category.Name == category)
            .ToListAsync();

        if (!items.Any())
            throw new EmptyResultException("No items");

        var result = new List<ItemModel>();
        foreach (var item in items)
        {
            result.Add(new ItemModel()
            {
                Article = item.Article,
                Brand = item.Brand.Name,
                Category = item.Category.Name,
                Descr = item.Descr,
                Name = item.Name,
                Stock = item.Stock,
                Id = item.Id
            });
        }

        return result;
    }

    public async Task PostNewItem(ItemModel model)
    {
        var brand = await _context.Brands.SingleOrDefaultAsync(x => x.Name == model.Brand);
        if (brand is null)
            throw new InvalidBrandException($"No Brand with name {brand}");

        var category = await _context.Categories.SingleOrDefaultAsync(x => x.Name == model.Category);
        if (category is null)
            throw new InvalidCategoryException($"No category with name {category}");

        await _context.AddAsync(new Item()
        {
            Article = model.Article,
            Brand = brand,
            BrandId = brand.Id,
            Category = category,
            CategoryId = category.Id,
            Descr = model.Descr,
            Name = model.Name,
            Stock = model.Stock
        });
        throw new NotImplementedException();
    }

    public async Task PostNewBrand(BrandModel model)
    {
        throw new NotImplementedException();
    }

    public async Task PostNewCategory(CategoryModel model)
    {
        throw new NotImplementedException();
    }

    public async Task Restock(string article, int count)
    {
        throw new NotImplementedException();
    }

    public async Task<ItemModel> AddItemToCart(ItemModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<ItemModel>> OrderItems(List<ItemModel> items)
    {
        throw new NotImplementedException();
    }
}
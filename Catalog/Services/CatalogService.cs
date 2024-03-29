using Catalog.Database;
using Catalog.Database.Entities;
using Catalog.Exceptions;
using Catalog.Models;
using Infrastructure.Models;
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

        var item = await _context.Items.SingleOrDefaultAsync(x => x.Article == model.Article);
        if (item is not null)
            throw new InvalidItemException($"Item with article '{model.Article} already exists'");

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
        await _context.SaveChangesAsync();
    }

    public async Task PostNewBrand(BrandModel model)
    {
        if (await _context.Brands.FirstOrDefaultAsync(x => x.Name == model.Name) is not null)
            throw new NotUniqueException("This Brand already exists");

        await _context.Brands.AddAsync(new Brand() {
            Name = model.Name,
            Descr = model.Descr,
            LogoURL = model.LogoURL
        });
        await _context.SaveChangesAsync();
    }

    public async Task PostNewCategory(CategoryModel model)
    {
        if (await _context.Categories.FirstOrDefaultAsync(x => x.Name == model.Name) is not null)
            throw new NotUniqueException("This category already exists");

        await _context.Categories.AddAsync(new Category() {
            Name = model.Name,
            Descr = model.Descr
        });
        await _context.SaveChangesAsync();
    }

    public async Task Restock(string article, int count)
    {
        var item = await _context.Items.SingleOrDefaultAsync(x => 
            x.Article == article
        );
        if (item is null)
            throw new ArgumentException($"Couldn't find item with article {article}");

        if (item.Stock + count > 500 || item.Stock + count < 0)
            throw new ArgumentException($"Stock ust be between 0 and 500, but got {item.Stock + count}");

        item.Stock += count;
        await _context.SaveChangesAsync();
    }

    public async Task<ItemModel> AddItemToCart(ItemModel model)
    {
        var item = await _context.Items.Include(x => x.Brand)
            .Include(x => x.Category)
            .SingleOrDefaultAsync(x => 
                x.Name == model.Name && 
                x.Article == model.Article
        );

        if (model.Stock < 1)
            throw new ArgumentException("You must add at least 1 item");

        if (item is null)
            throw new InvalidItemException($"Couldn't find item with Name {model.Name} and Article {model.Article}");

        if (item.Stock - model.Stock < 0)
            throw new OutOfStockException($"Have only {item.Stock}, but requested {model.Stock}");

        return new ItemModel() {
            Id = item.Id,
            Name = item.Name,
            Article = item.Article,
            Descr = item.Descr,
            Brand = item.Brand.Name,
            Category = item.Category.Name,
            Stock = model.Stock
        };
    }

    public async Task<ICollection<ItemModel>> OrderItems(List<ItemModel> items)
    {
        var res = new List<ItemModel>();
        foreach (var model in items)
        {
            var item = await _context.Items.Include(x => x.Brand)
            .Include(x => x.Category)
            .SingleOrDefaultAsync(x => 
                x.Name == model.Name && 
                x.Article == model.Article
            );

            if (model.Stock < 1)
                throw new ArgumentException("You must add at least 1 item");

            if (item is null)
                throw new InvalidItemException($"Couldn't find item with Name {model.Name} and Article {model.Article}");

            if (item.Stock - model.Stock < 0)
                throw new OutOfStockException($"Have only {item.Stock}, but requested {model.Stock}");

            res.Add(new ItemModel() {
                Id = item.Id,
                Name = item.Name,
                Article = item.Article,
                Descr = item.Descr,
                Brand = item.Brand.Name,
                Category = item.Category.Name,
                Stock = model.Stock
            });
            item.Stock -= model.Stock;
        }

        await _context.SaveChangesAsync();
        return res;
    }

    public async Task<ItemsCheck> CheckItems(ItemsToCheckList toChecks)
    {
        if (!toChecks.Items.Any()) 
            throw new EmptyItemsListException("Items list is empty");
        var res = new List<ItemCheck>();
        foreach (var model in toChecks.Items)
        {
            var item = await _context.Items.Include(x => x.Brand)
                .Include(x => x.Category)
                .SingleOrDefaultAsync(x =>
                    x.Article == model.Article &&
                    x.Name == model.Name &&
                    x.Brand.Name == model.Brand &&
                    x.Category.Name == model.Category &&
                    x.Stock >= model.Stock);
            if (item is null)
            {
                res.Add(new ItemCheck()
                {
                    Article = model.Article,
                    Brand = model.Brand,
                    Category = model.Category,
                    Name = model.Name,
                    Id = 0,
                    IsCorrect = false,
                    RequestingCount = model.Stock,
                    Stock = 0
                });
            }
            else
            {
                res.Add(new ItemCheck()
                {
                    Article = item.Article,
                    Brand = item.Brand.Name,
                    Category = item.Category.Name,
                    Id = item.Id,
                    IsCorrect = item.Stock >= model.Stock,
                    Name = item.Name,
                    RequestingCount = model.Stock,
                    Stock = item.Stock
                });
            }
        }
        return new ItemsCheck() {Items = res};
    }

    public async Task<ItemCheck> CheckItem(ItemToCheck toCheck)
    {
        ItemCheck res;
        var item = await _context.Items.Include(x => x.Brand)
            .Include(x => x.Category)
            .SingleOrDefaultAsync(x =>
                x.Article == toCheck.Article &&
                x.Name == toCheck.Name &&
                x.Brand.Name == toCheck.Brand &&
                x.Category.Name == toCheck.Brand &&
                x.Stock <= toCheck.Stock);
        
        if (item is null)
        {
            res = new ItemCheck()
            {
                Article = toCheck.Article,
                Brand = toCheck.Brand,
                Category = toCheck.Category,
                Name = toCheck.Name,
                Id = 0,
                IsCorrect = false,
                RequestingCount = toCheck.Stock,
                Stock = 0
            };
        }
        else
        {
            res = new ItemCheck()
            {
                Article = item.Article,
                Brand = item.Brand.Name,
                Category = item.Category.Name,
                Id = item.Id,
                IsCorrect = item.Stock >= toCheck.Stock,
                Name = item.Name,
                RequestingCount = toCheck.Stock,
                Stock = item.Stock
            };
        }

        return res;
    }
}
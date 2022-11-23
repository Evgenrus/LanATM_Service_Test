using Catalog.Database;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Services;

public class RabbitmqService : IRabbitmqService
{
    private CatalogDbContext _context;

    public RabbitmqService(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<RestockRespList> OrderItems(RestockRequestList items)
    {
        var res = new RestockRespList() {Items = new List<RestockResp>()};
        foreach (var request in items.Items)
        {
            var item = await _context.Items.SingleOrDefaultAsync(x => 
                x.Article == request.Article
            );
            if (item is null)
            {
                res.Items.Add(new RestockResp()
                {
                    ErrMsg = $"Couldn't find item with article {request.Article}", Stock = 0, Requested = request.Change
                });
                continue;
            }

            if (item.Stock - request.Change > 500 || item.Stock - request.Change < 0)
            {
                res.Items.Add(new RestockResp()
                {
                    ErrMsg = $"Stock ust be between 0 and 500, but got {item.Stock + request.Change}",
                    Stock = 0 - request.Change, Requested = request.Change
                });
                continue;
            }

            item.Stock -= request.Change;
            await _context.SaveChangesAsync();
            res.Items.Add(new RestockResp() { ErrMsg = "", Requested = request.Change, Stock = item.Stock });
        }

        return res;
    }

    public async Task<RestockResp> Restock(RestockRequest request)
    {
        var item = await _context.Items.SingleOrDefaultAsync(x => 
            x.Article == request.Article
        );
        if (item is null)
            return new RestockResp()
            {
                ErrMsg = $"Couldn't find item with article {request.Article}", Stock = 0, Requested = request.Change
            };

        if (item.Stock + request.Change > 500 || item.Stock + request.Change < 0)
            return new RestockResp()
            {
                ErrMsg = $"Stock ust be between 0 and 500, but got {item.Stock + request.Change}", Stock = 0 - request.Change, Requested = request.Change
            };

        item.Stock += request.Change;
        await _context.SaveChangesAsync();
        return new RestockResp() { ErrMsg = "", Requested = request.Change, Stock = item.Stock };
    }
}
using Catalog.Database;
using Infrastructure.Models;
using MassTransit.Internals.Caching;
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
            if (request.Change < 1)
            {
                res.Items.Add(new RestockResp()
                {
                    ErrMsg = "For restock use Method restock", Stock = 0
                });
                continue;
            }
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

    public async Task<RestockRespList> Restock(RestockRequestList request)
    {
        var res = new List<RestockResp>();
        foreach (var reqItem in request.Items)
        {
            var item = await _context.Items.SingleOrDefaultAsync(x =>
                x.Article == reqItem.Article
            );
            if (item is null)
                res.Add(new RestockResp()
                {
                    ErrMsg = $"Couldn't find item with article {reqItem.Article}", Stock = 0, Requested = reqItem.Change
                });

            if (item.Stock + reqItem.Change > 500 || item.Stock - reqItem.Change < 0)
                res.Add(new RestockResp()
                {
                    ErrMsg = $"Stock ust be between 0 and 500, but got {item.Stock + reqItem.Change}",
                    Stock = 0 - reqItem.Change, Requested = reqItem.Change
                });

            item.Stock += reqItem.Change;
            await _context.SaveChangesAsync();
            res.Add(new RestockResp() { ErrMsg = "", Requested = reqItem.Change, Stock = item.Stock });
        }

        return new RestockRespList() {Items = res};
    }
}
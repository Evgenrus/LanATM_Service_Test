using Catalog.Services;
using Infrastructure.Models;
using MassTransit;

namespace Catalog.Consumers;

public class ItemModelCheckConsumer : IConsumer<ItemToCheck>
{
    private ICatalogService _service;

    public ItemModelCheckConsumer(ICatalogService service)
    {
        _service = service;
    }
    
    public async Task Consume(ConsumeContext<ItemToCheck> context)
    {
        var content = context.Message;
        var res = await _service.CheckItem(content);
        await context.RespondAsync(res);
    }
}
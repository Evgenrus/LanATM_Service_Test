using Catalog.Models;
using Catalog.Services;
using Infrastructure.Models;
using MassTransit;

namespace Catalog.Consumers;

public class ItemsCheckConsumer : IConsumer<IEnumerable<ItemModel>>
{
    private ICatalogService _service;

    public ItemsCheckConsumer(ICatalogService service)
    {
        _service = service;
    }
    
    public async Task Consume(ConsumeContext<IEnumerable<ItemModel>> context)
    {
        var content = context.Message;
        var res = await _service.CheckItems(content);
        await context.RespondAsync(res);
    }
}
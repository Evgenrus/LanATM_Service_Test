using Catalog.Models;
using Catalog.Services;
using Infrastructure.Models;
using MassTransit;

namespace Catalog.Consumers;

public class ItemsCheckConsumer : IConsumer<List<ItemModel>>
{
    private ICatalogService _service;

    public ItemsCheckConsumer(ICatalogService service)
    {
        _service = service;
    }
    
    public async Task Consume(ConsumeContext<List<ItemModel>> context)
    {
        var content = context.Message;
        var res = await _service.CheckItems(content);
        await context.RespondAsync(res);
    }
}
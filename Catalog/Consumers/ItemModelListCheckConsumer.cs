using Catalog.Models;
using Catalog.Services;
using MassTransit;

namespace Catalog.Consumers;

public class ItemModelListCheckConsumer : IConsumer<Infrastructure.Models.ItemsToCheckList>
{
    private ICatalogService _service;

    public ItemModelListCheckConsumer(ICatalogService service)
    {
        _service = service;
    }
    
    public async Task Consume(ConsumeContext<Infrastructure.Models.ItemsToCheckList> context)
    {
        var content = context.Message;
        var res = await _service.CheckItems(content);
        await context.RespondAsync(res);
    }
}
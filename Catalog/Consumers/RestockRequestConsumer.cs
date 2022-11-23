using Catalog.Services;
using Infrastructure.Models;
using MassTransit;

namespace Catalog.Consumers;

public class RestockRequestConsumer : IConsumer<RestockRequest>
{
    private IRabbitmqService _service;
    private IBusControl _bus;

    public RestockRequestConsumer(IRabbitmqService service, IBusControl bus)
    {
        _service = service;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<RestockRequest> context)
    {
        try
        {
            var message = context.Message;
            var res = await _service.Restock(message);
            await context.RespondAsync(res);
        }
        catch (Exception e)
        {
            await context.RespondAsync(new FinishOrderResponse()
            {
                Id = -1,
                ErrMsg = e.Message
            });
        }
    }
}
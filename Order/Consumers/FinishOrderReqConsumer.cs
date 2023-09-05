using Infrastructure.Models;
using MassTransit;
using Order.Services;

namespace Order.Consumers;

public class FinishOrderReqConsumer : IConsumer<FinishOrderRequest>
{
    private IBusControl _bus;
    private IRabbitmqService _service;

    public FinishOrderReqConsumer(IBusControl bus, IRabbitmqService service)
    {
        _bus = bus;
        _service = service;
    }

    public async Task Consume(ConsumeContext<FinishOrderRequest> context)
    {
        try
        {
            var message = context.Message;
            var res = await _service.FinishOrder(message);
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
using Delivery.Service;
using Infrastructure.Models;
using MassTransit;

namespace Delivery.Consumers;

public class DeliveryCancelConsumer : IConsumer<DeliveryCancelRequest>
{
    private IBusControl _bus;
    private IRabbitmqService _service;

    public DeliveryCancelConsumer(IRabbitmqService service, IBusControl bus)
    {
        _service = service;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<DeliveryCancelRequest> context)
    {
        try
        {
            var message = context.Message;
            var res = await _service.CancelDelivery(message);
            await context.RespondAsync(res);
        }
        catch (Exception e)
        {
            await context.RespondAsync(new DeliveryResponse()
            {
                DeliveryId = -1,
                ErrMsg = e.Message
            });
        }
    }
}
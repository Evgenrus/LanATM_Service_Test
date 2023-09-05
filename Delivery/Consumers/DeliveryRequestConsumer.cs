using Delivery.Service;
using Infrastructure.Models;
using MassTransit;

namespace Delivery.Consumers;

public class DeliveryRequestConsumer : IConsumer<DeliveryRequest>
{
    private IRabbitmqService _service { get; set; }
    
    public DeliveryRequestConsumer(IRabbitmqService service)
    {
        _service = service;
    }

    
    public async Task Consume(ConsumeContext<DeliveryRequest> context)
    {
        try
        {
            var message = context.Message;
            var res = await _service.PostNewDelivery(message);
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
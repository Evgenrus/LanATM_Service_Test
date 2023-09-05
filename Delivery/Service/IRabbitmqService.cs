using Infrastructure.Models;

namespace Delivery.Service;

public interface IRabbitmqService
{
    public Task<DeliveryResponse> PostNewDelivery(DeliveryRequest delivery);
    public Task<DeliveryCancelAnswer> CancelDelivery(DeliveryCancelRequest request);
}
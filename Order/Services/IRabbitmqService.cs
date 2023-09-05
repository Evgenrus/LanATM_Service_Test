using Infrastructure.Models;

namespace Order.Services;

public interface IRabbitmqService
{
    public Task<FinishOrderResponse> FinishOrder(FinishOrderRequest request);
}
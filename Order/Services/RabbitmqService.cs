using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Order.Database;

namespace Order.Services;

public class RabbitmqService : IRabbitmqService
{
    private OrderDbContext _context;

    public RabbitmqService(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<FinishOrderResponse> FinishOrder(FinishOrderRequest request)
    {
        var order = await _context.Orders.SingleOrDefaultAsync(x => x.Id == request.Id);
        if (order is null)
            return new FinishOrderResponse() { Id = -1, ErrMsg = "Wrong order id" };
        if (order.IsCanceled)
            return new FinishOrderResponse() { Id = -1, ErrMsg = "Order is canceled" };
        if (order.IsFinished)
            return new FinishOrderResponse() { Id = -1, ErrMsg = "This order is already finished" };
        order.IsFinished = true;
        order.IsOnDelivery = false;
        await _context.SaveChangesAsync();

        return new FinishOrderResponse() { Id = order.Id, ErrMsg = "" };
    }
}
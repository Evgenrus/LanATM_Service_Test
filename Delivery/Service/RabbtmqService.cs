using Delivery.Database;
using Delivery.Database.Entities;
using Delivery.Exceptions;
using Delivery.Models;
using Infrastructure.Helpers;
using Infrastructure.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Service;

public class RabbtmqService : IRabbitmqService
{
    private readonly DeliveryDbContext _context;
    private IBusControl _bus;

    public RabbtmqService(IBusControl bus, DeliveryDbContext context)
    {
        _bus = bus;
        _context = context;
    }

    public async Task<DeliveryResponse> PostNewDelivery(DeliveryRequest delivery)
    {
        if (!delivery.Items.Items.Any())
            throw new EmptyDeliveryException("Order doesn't contains any items");

        var models = new ItemsToCheckList()
        {
            Items = new List<ItemToCheck>()
        };

        foreach (var deliveryItem in delivery.Items.Items)
        {
            models.Items.Add(new ItemToCheck
            {
                Article = deliveryItem.Article,
                Brand = deliveryItem.Brand,
                Category = deliveryItem.Category,
                Descr = deliveryItem.Descr,
                Name = deliveryItem.Name,
                Stock = deliveryItem.Stock, Id = deliveryItem.Id
            });
        }

        var items = new List<DeliveryItem>();
        foreach (var item in delivery.Items.Items)
        {
            items.Add(new DeliveryItem()
            {
                Article = item.Article,
                Brand = item.Brand,
                Category = item.Category,
                Count = item.Stock,
                Descr = item.Descr,
                Name = item.Name
            });
        }

        var addressModel = AddressModel.FromString(delivery.Address);

        var address = await _context.Addresses.Include(x => x.Customer)
            .SingleOrDefaultAsync(x =>
            x.Region == addressModel.Region &&
            x.City == addressModel.City &&
            x.District == addressModel.District &&
            x.Street == addressModel.Street &&
            x.House == addressModel.House &&
            x.Floor == addressModel.Floor &&
            x.Flat == addressModel.Flat &&
            x.Customer.Login == delivery.CustomerLogin);
        if (address is null)
            throw new InvalidAddressException("Couldnt find this address");

        var deliveryResp = new OrderDelivery()
        {
            Address = address,
            AddressId = address.Id,
            Courier = null,
            CourierId = null,
            Items = items,
            Status = Status.Pending,
            OrderId = delivery.Id
        };
        
        await _context.Deliveries.AddAsync(deliveryResp);

        await _context.SaveChangesAsync();
        return new DeliveryResponse()
        {
            DeliveryId = deliveryResp.Id,
        };
    }
    
    public async Task<DeliveryCancelAnswer> CancelDelivery(DeliveryCancelRequest request)
    {
        var delivery = await _context.Deliveries.SingleOrDefaultAsync(x => x.Id == request.DeliveryId);
        if (delivery is null)
            return new DeliveryCancelAnswer() { ErrMsg = "This delivery doesn't exists", Id = -1 };
        if (delivery.Status == Status.Canceled)
            return new DeliveryCancelAnswer() { ErrMsg = "This delivery is already canceled", Id = -1 };
        if (delivery.Status == Status.Delivered)
            return new DeliveryCancelAnswer() { ErrMsg = "This delivery is already delivered", Id = -1 };

        delivery.Courier = null;
        delivery.CourierId = null;
        delivery.Status = Status.Canceled;
        await _context.SaveChangesAsync();

        return new DeliveryCancelAnswer() { Id = delivery.Id, ErrMsg = ""};
    }
}
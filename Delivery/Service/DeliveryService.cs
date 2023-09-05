using Delivery.Database;
using Delivery.Database.Entities;
using Delivery.Exceptions;
using Delivery.Models;
using Infrastructure.Helpers;
using Infrastructure.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace Delivery.Service;

public class DeliveryService : IDeliveryService
{
    private readonly DeliveryDbContext _context;
    private IRequestClient<ItemsCheck> _request;
    private IBusControl _bus;

    public DeliveryService(DeliveryDbContext context, IRequestClient<ItemsCheck> request, IBusControl bus)
    {
        _context = context;
        _request = request;
        _bus = bus;
    }

    public async Task<ICollection<DeliveryModel>> GetAllFreeOrders()
    {
        var orders = await _context.Deliveries.Include(x => x.Address)
            .ThenInclude(x => x.Customer)
            .Where(x => x.Status == Status.Pending)
            .ToListAsync();
        if (orders is null)
            throw new NoPendingDeliveries("Currently no free orders");

        var res = new List<DeliveryModel>();
        foreach (var order in orders)
        {
            res.Add(new DeliveryModel()
            {
                Address = order.Address.ToString()!,
                CourierId = null,
                CourierName = null,
                CustomerLogin = order.Address.Customer.Login,
                CustomerName = Customer.GetName(order.Address.Customer),
                Id = order.Id,
                Status = order.Status.ToString(),
                OrderId = order.OrderId
            });
        }

        return res;
    }

    public async Task BeginOrderDelivery(int orderId, int courierId)
    {
        if (orderId < 1)
            throw new ArgumentException("Order id must be >= 1");
        if (courierId < 1)
            throw new ArgumentException("Wrong courier id");
        var order = await _context.Deliveries.SingleOrDefaultAsync(x => 
            x.Id == orderId &&
            x.Status == Status.Pending
        );
        if (order is null)
            throw new InvalidDeliveryException("couldn't find this delivery");

        var courier = await _context.Couriers.SingleOrDefaultAsync(x => x.Id == courierId);

        order.Status = Status.Assigned;
        order.CourierId = courierId;
        order.Courier = courier;
        await _context.SaveChangesAsync();
    }

    public async Task ReceiveOrderFromStock(int orderId, int courierId)
    {
        if (orderId < 1)
            throw new ArgumentException("order id must be >= 1");
        if (courierId < 1)
            throw new ArgumentException("Wrong courier id");
        var order = await _context.Deliveries.SingleOrDefaultAsync(x => 
            x.Id == orderId &&
            x.CourierId == courierId &&
            x.Status == Status.Assigned
        );
        if (order is null)
            throw new InvalidDeliveryException("couldn't find this delivery");

        order.Status = Status.InProcess;
        await _context.SaveChangesAsync();
    }

    public async Task FinishDelivery(int orderId, int courierId)
    {
        if (orderId < 1)
            throw new ArgumentException("order id must be >= 1");
        if (courierId < 1)
            throw new ArgumentException("Wrong courier id");
        var order = await _context.Deliveries.SingleOrDefaultAsync(x => 
            x.Id == orderId &&
            x.CourierId == courierId &&
            x.Status == Status.InProcess
        );
        if (order is null)
            throw new InvalidDeliveryException("couldn't find this delivery");

        order.Status = Status.Delivered;
        var res = await RabbitMQHelpers
            .GetResponseRabbitTask<FinishOrderRequest, FinishOrderResponse>(
                _bus,
                new FinishOrderRequest() { Id = orderId }, 
                new Uri("rabbitmq://localhost/order-finish-queue1")
                );
        if (res.Id < 1)
            throw new Exception(res.ErrMsg);
        await _context.SaveChangesAsync();
    }

    public async Task<string> DeliveryStatus(int orderId)
    {
        if (orderId < 1)
            throw new ArgumentException("order id must be >= 1");
        var order = await _context.Deliveries.SingleOrDefaultAsync(x => x.Id == orderId);
        if (order is null)
            throw new InvalidDeliveryException("Couldn't find this delivery");

        return order.Status.ToString();
    }

    public async Task<ICollection<AddressModel>> GetAddressesByCustomerId(int id)
    {
        if (id < 1)
            throw new ArgumentException("Wrong customer id");

        var customer = await _context.Customers.Include(x => x.Addresses)
            .SingleOrDefaultAsync(x => x.Id == id);
        if (customer is null)
            throw new InvalidCustomerException($"Couldn't find customer with id {id}");

        if (!customer.Addresses.Any())
            throw new NoAddressesException("This user hasn't add any addresses");

        var res = new List<AddressModel>();
        foreach (var address in customer.Addresses)
        {
            res.Add(new AddressModel()
            {
                Region = address.Region,
                City = address.City,
                District = address.District,
                Street = address.Street,
                House = address.House,
                Floor = address.Floor,
                Flat = address.Flat,
                Id = address.Id
            });
        }

        return res;
    }

    public async Task<int> AddNewCustomer(CustomerModel model)
    {
        var collision = await _context.Customers.Where(x => 
            x.Login == model.Login || 
            x.Mail == model.Mail)
            .ToListAsync();
        if (collision.Any())
            throw new CustomerAlreadyExistsException("this customer already exists");

        var customer = new Customer()
        {
            Addresses = new List<Address>(),
            Deliveries = new List<OrderDelivery>(),
            Login = model.Login,
            Mail = model.Mail,
            Name = model.Name,
            Phone = model.Phone,
            Surname = model.Surname
        };

        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();

        return customer.Id;
    }

    public async Task<int> AddNewCourier(CourierModel model)
    {
        var collision = await _context.Couriers.Where(x => 
                x.Login == model.Login || 
                x.Mail == model.Mail)
            .ToListAsync();
        if (collision.Any())
            throw new CourierAlreadyExistsException("this courier already exists");

        var courier = new Courier()
        {
            Deliveries = new List<OrderDelivery>(),
            Login = model.Login,
            Mail = model.Mail,
            Name = model.Name,
            Phone = model.Phone,
            Surname = model.Surname,
            Photo = model.Photo
        };

        await _context.Couriers.AddAsync(courier);
        await _context.SaveChangesAsync();

        return courier.Id;
    }

    public async Task<int> PostNewAddress(AddressModel model)
    {
        if (model.CustomerId is null)
            throw new ArgumentException("Wrong customer Id");
        var customer = await _context.Customers.SingleOrDefaultAsync(x => x.Id == model.CustomerId);
        if (customer is null)
            throw new InvalidCustomerException($"Couldn't find customer with id {model.CustomerId}");
        
        var collision = await _context.Addresses.Where(x => 
            x.Region == model.Region &&
            x.City == model.City &&
            x.District == model.District &&
            x.Street == model.Street &&
            x.House == model.House &&
            x.Floor == model.Floor &&
            x.Flat == model.Flat &&
            x.CustomerId == model.CustomerId)
            .ToListAsync();
        if (collision.Any())
            throw new AddressAlreadyExists("This address already exists");

        var addr = new Address()
        {
            Region = model.Region,
            City = model.City,
            District = model.District,
            Street = model.Street,
            House = model.House,
            Floor = model.Floor,
            Flat = model.Flat,
            CustomerId = model.CustomerId.Value,
            Customer = customer,
            Deliveries = new List<OrderDelivery>()
        };

        await _context.Addresses.AddAsync(addr);
        await _context.SaveChangesAsync();

        return addr.Id;
    }
}
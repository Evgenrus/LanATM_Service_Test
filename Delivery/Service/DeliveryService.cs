using Delivery.Database;
using Delivery.Database.Entities;
using Delivery.Exceptions;
using Delivery.Models;
using Infrastructure.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace Delivery.Service;

public class DeliveryService : IDeliveryService
{
    private readonly DeliveryDbContext _context;
    private readonly IRequestClient<List<ItemModel>> _request;

    public DeliveryService(DeliveryDbContext context, IRequestClient<List<ItemModel>> request)
    {
        _context = context;
        _request = request;
    }
    
    public async Task PostNewDelivery(DeliveryModel delivery)
    {
        if (!delivery.Items.Any())
            throw new EmptyDeliveryException("Order doesn't contains any items");

        var check = new List<ItemCheck>();
        using (var resp = _request.Create(delivery.Items))
        {
            var p = await resp.GetResponse<List<ItemCheck>>();
            check = p.Message;
        }

        if (!check.Any())
            throw new Exception();

        var items = new List<DeliveryItem>();
        foreach (var item in delivery.Items)
        {
            items.Add(new DeliveryItem()
            {
                Article = item.Article,
                Brand = item.Brand,
                Category = item.Category,
                Count = item.Count,
                Descr = item.Descr,
                Name = item.Name
            });
        }

        var addressModel = AddressModel.FromString(delivery.Address);

        var address = await _context.Addresses.SingleOrDefaultAsync(x =>
            x.Region == addressModel.Region &&
            x.City == addressModel.City &&
            x.District == addressModel.District &&
            x.Street == addressModel.Street &&
            x.House == addressModel.House &&
            x.Floor == addressModel.Floor &&
            x.Flat == addressModel.Flat);
        if (address is null)
            throw new InvalidAddressException("Couldnt find this address");

        await _context.Deliveries.AddAsync(new OrderDelivery()
        {
            Address = address,
            AddressId = address.Id,
            Courier = null,
            CourierId = null,
            Items = items,
            Status = Status.Pending
        });

        await _context.SaveChangesAsync();
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
                CustomerId = order.Address.CustomerId,
                CustomerName = Customer.GetName(order.Address.Customer),
                Id = order.Id,
                Status = order.Status.ToString()
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
            throw new InvalidDeliveryException();

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
            throw new InvalidDeliveryException();

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
            throw new InvalidDeliveryException();

        order.Status = Status.Delivered;
        await _context.SaveChangesAsync();
    }

    public async Task<string> DeliveryStatus(int orderId)
    {
        if (orderId < 1)
            throw new ArgumentException("order id must be >= 1");
        var order = await _context.Deliveries.SingleOrDefaultAsync(x => x.Id == orderId);
        if (order is null)
            throw new InvalidDeliveryException();

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
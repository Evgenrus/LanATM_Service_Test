using System.Net.Http.Headers;
using System.Collections.Generic;
using System;
using Order.Database;
using System.Collections;
using Order.Models;
using Order.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Infrastructure.Helpers;
using Infrastructure.Models;
using MassTransit;
using Order.Exceptions;

namespace Order.Services;
public class OrderService : IOrderService
{
    private OrderDbContext _context { get; set; }
    private IBusControl _bus { get; set; }

    public OrderService(OrderDbContext context, IBusControl bus)
    {
        _context = context;
        _bus = bus;
    }

    public async Task<CartModel> GetCartById(int id)
    {
        if(id < 0) 
            throw new ArgumentException("Id must be >= 0");

        var cart = await _context.Carts.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == id);
        if (cart is null)
            throw new InvalidCartException($"Couldn't find cart with id {id}");

        var res = new CartModel() {
            Id = cart.Id,
            Items = new List<ItemModel>()
        };

        foreach (var item in cart.Items)
        {
            res.Items.Add(new ItemModel() {
                Name = item.Name,
                Article = item.Article,
                Descr = item.Descr,
                Brand = item.Brand,
                Category = item.Category,
                Count = item.Count
            });
        }

        return res;
    }

    public async Task<ICollection<CartModel>> GetCartsByCustomerId(int id)
    {
        if (id < 0)
            throw new ArgumentException("Id must be >= 0");

        var carts = await _context.Carts.Include(x => x.Items).Where(x => x.CustomerId == id).ToListAsync();
        if (carts is null)
            throw new InvalidCartException("");

        var res = new List<CartModel>(); 

        foreach (var cart in carts)
        {
            var items = new List<ItemModel>();
            foreach (var item in cart.Items)
            {
                items.Add(new ItemModel() {
                Name = item.Name,
                Article = item.Article,
                Descr = item.Descr,
                Brand = item.Brand,
                Category = item.Category,
                Count = item.Count
                });
            }
            res.Add(new CartModel() {
                Id = cart.Id,
                Items = items
            });
        }

        return res;
    }

    public async Task<OrderModel> GetOrderById(int id)
    {
        if(id < 0) 
            throw new ArgumentException("Id must be >= 0");

        var order = await _context.Orders.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == id);
        if (order is null)
            throw new InvalidOrderException("");

        var res = new OrderModel() {
            Id = order.Id,
            IsCanceled = order.IsCanceled,
            IsFinished = order.IsFinished,
            IsOnDelivery = order.IsOnDelivery,
            Items = new List<ItemModel>(),
            DeliveryId = order.DeliveryId
        };

        foreach (var item in order.Items)
        {
            res.Items.Add(new ItemModel() {
                Name = item.Name,
                Article = item.Article,
                Descr = item.Descr,
                Brand = item.Brand,
                Category = item.Category,
                Count = item.Count
            });
        }

        return res;
    }

    public async Task<ICollection<OrderModel>> GetOrdersByCustomerId(int id)
    {
        if (id < 0)
            throw new ArgumentException("Id must be >= 0");

        var orders = await _context.Orders.Include(x => x.Items).Where(x => x.CustomerId == id).ToListAsync();
        if (orders is null)
            throw new InvalidOrderException("");

        var res = new List<OrderModel>(); 

        foreach (var order in orders)
        {
            var items = new List<ItemModel>();
            foreach (var item in order.Items)
            {
                items.Add(new ItemModel() {
                Name = item.Name,
                Article = item.Article,
                Descr = item.Descr,
                Brand = item.Brand,
                Category = item.Category,
                Count = item.Count
                });
            }
            res.Add(new OrderModel() {
                Id = order.Id,
                Items = items,
                IsCanceled = order.IsCanceled,
                IsFinished = order.IsFinished,
                IsOnDelivery = order.IsOnDelivery,
                DeliveryId = order.DeliveryId
            });
        }

        return res;
    }

    public async Task<OrderModel> PostOrder(PostOrderModel model)
    {
        if(model.CustomerId < 0)
            throw new ArgumentException("Id must be >= 0");

        var cart = await _context.Carts.Include(x => x.Items).SingleOrDefaultAsync(x => x.CustomerId == model.CustomerId && x.Id == model.CartId);
        if (cart is null)
            throw new InvalidCartException("No such cart");

        if (cart.Items is null)
            throw new InvalidOrderException("Order can't be empty");

        var cartItems = cart.Items.Select(item => new ItemModel()
        {
            Article = item.Article,
            Brand = item.Brand,
            Category = item.Category,
            Count = item.Count,
            Descr = item.Descr,
            Name = item.Name
        }).ToList();

        var customer = await _context.Customers.FindAsync(model.CustomerId);
        if(customer is null)
            throw new InvalidCustomerException("couldn't find customer with such Id");

        var check = new ItemsToCheckList() {Items = new List<ItemToCheck>()};
        foreach (var modelItem in cart.Items)
        {
            check.Items.Add(new ItemToCheck()
            {
                Article = modelItem.Article,
                Brand = modelItem.Brand,
                Category = modelItem.Category,
                Descr = modelItem.Descr,
                Name = modelItem.Name,
                Stock = modelItem.Count,
                Id = 0
            });
        }

        var response = await RabbitMQHelpers.GetResponseRabbitTask<ItemsToCheckList, ItemsCheck>(_bus, check,
            new Uri("rabbitmq://localhost/items-queue1"));
        foreach (var itemCheck in response.Items)
        {
            if (!itemCheck.IsCorrect)
                throw new ItemCheckFailedException("wrong");
        }

        if (response.Items.Any(responseItem => responseItem.IsCorrect == false))
        {
            throw new ItemCheckFailedException("Items check has failed");
        }

        var items = cart.Items.Select(item => new OrderItem()
            {
                Name = item.Name,
                Article = item.Article,
                Descr = item.Descr,
                Brand = item.Brand,
                Category = item.Category,
                Count = item.Count
            })
            .ToList();
        var order = new Orders()
        {
            Customer = customer,
            CustomerId = customer.Id,
            Items = items,
            IsCanceled = false,
            IsOnDelivery = model.IsOnDelivery,
            IsFinished = false,
        };
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        var orderModel = new OrderModel()
        {
            Address = model.Address,
            Id = order.Id,
            IsCanceled = false,
            IsFinished = false,
            IsOnDelivery = model.IsOnDelivery,
            Items = cartItems
        };
        
        if (model.IsOnDelivery)
        {
            var deliveryRequest = new DeliveryRequest()
            {
                Address = model.Address!,
                CustomerLogin = customer.Login,
                CustomerName = customer.Name,
                Items = check,
                Id = order.Id
            };
            var postDelivery = await RabbitMQHelpers.GetResponseRabbitTask<DeliveryRequest, DeliveryResponse>(
                _bus,
                deliveryRequest,
                new Uri("rabbitmq://localhost/delivery-post-queue1")
            );
            if (postDelivery.DeliveryId < 1)
            {
                order.IsCanceled = true;
                await _context.SaveChangesAsync();
                throw new PostDeliveryFailedException(postDelivery.ErrMsg ?? "Unknown error");
            }

            order.DeliveryId = deliveryRequest.Id;
            orderModel.DeliveryId = deliveryRequest.Id;
            var restockitems = new List<RestockRequest>();
            foreach (var orderItem in items)
            {
                restockitems.Add(new RestockRequest
                {
                    Article = orderItem.Article, Change = orderItem.Count
                });
            }

            var restock = await RabbitMQHelpers
                .GetResponseRabbitTask<RestockRequestList, RestockRespList>(
                    _bus,
                    new RestockRequestList() {Items = restockitems},
                    new Uri("rabbitmq://localhost/items-order-queue1")
                    );

            await _context.SaveChangesAsync();
        }
        return orderModel;
    }

    public async Task<CartModel> NewCart(int customerId)
    {
        if (customerId < 0)
            throw new ArgumentException("Id must be >= 0");

        var customer = await _context.Customers.FindAsync(customerId);
        if (customer is null)
            throw new InvalidCustomerException($"Couldn't find user with {customerId}");

        var res = new Carts() {
            CustomerId = customerId,
            Customer = customer,
            Items = new List<CartItem>()
        };

        await _context.AddAsync(res);
        await _context.SaveChangesAsync();

        return new CartModel() {
            Id = res.Id,
        };
    }

    public async Task<CartModel> AddItemToCart(ItemModel model, int cartId, int customerId)
    {
        if (cartId < 1 || customerId < 0)
            throw new ArgumentException("Id must be >= 0");

        var cart = await _context.Carts.Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.CustomerId == customerId && 
                x.Id == cartId
            );
        if (cart is null)
            throw new InvalidCartException("Couldn't find cart");

        var item = new CartItem() {
            Name = model.Name,
            Article = model.Article,
            Descr = model.Descr,
            Brand = model.Brand,
            Category = model.Category,
            Count = model.Count,
            Carts = cart,
            CartsId = cart.Id
        };
        
        cart.Items.Add(item);
        await _context.CartItems.AddAsync(item);
        await _context.SaveChangesAsync();

        var itemmodels = cart.Items.Select(prod => new ItemModel()
            {
                Name = prod.Name,
                Article = prod.Article,
                Descr = prod.Descr,
                Brand = prod.Brand,
                Category = prod.Category,
                Count = prod.Count
            })
            .ToList();

        return new CartModel() {
            Id = cart.Id,
            Items = itemmodels
        };
    }

    public async Task CancelOrder(int orderId)
    {
        if (orderId < 0)
            throw new ArgumentException("OrderId should be >= 0");

        var order = await _context.Orders.Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.Id == orderId);
        if (order is null)
            throw new InvalidOrderException("No such order");
        if (order.IsCanceled == true)
            throw new AlreadyCanceledException("This order is canceled already");


        var res = await RabbitMQHelpers
            .GetResponseRabbitTask<DeliveryCancelRequest, DeliveryCancelAnswer>(
                _bus,
                new DeliveryCancelRequest() { DeliveryId = order.Id },
                new Uri("rabbitmq://localhost/delivery-cancel-queue1")
                );
        if (res.Id < 1)
            throw new CancelDeliveryFailedException(res.ErrMsg ?? "Unknown error");

        order.IsCanceled = true;

        var restocklist = new List<RestockRequest>();
        foreach (var item in order.Items)
        {
            restocklist.Add(new RestockRequest()
            {
                Article = item.Article,
                Change = item.Count
            });
        }

        var restock = await RabbitMQHelpers
            .GetResponseRabbitTask<RestockRequestList, RestockRespList>(
                _bus,
                new RestockRequestList() {Items = restocklist},
                new Uri("rabbitmq://localhost/items-restock-queue1")
            );

        foreach (var resp in restock.Items)
        {
            if (!string.IsNullOrEmpty(resp.ErrMsg))
                throw new InvalidItemException(resp.ErrMsg);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<CustomerModel> GetCustomerByLogin(string Login)
    {
        if(string.IsNullOrWhiteSpace(Login))
            throw new ArgumentException("Login is null, empty or contains only whitespaces");

        var customer = await _context.Customers.SingleOrDefaultAsync(x => x.Login == Login);
        if (customer is null)
            throw new InvalidCustomerException($"Couldn't find user with login {Login}");

        return new CustomerModel() {
            Id = customer.Id,
            Login = customer.Login,
            Name = customer.Name,
            Surname = customer.Surname,
            Phone = customer.Phone,
            Mail = customer.Mail
        };
    }

    public async Task<int> PostNewCustomer(CustomerModel model)
    {
        var collision = await _context.Customers.SingleOrDefaultAsync(x => x.Login == model.Login);
        if (collision is not null)
            throw new AlreadyRegisteredException("This user is already exists");

        var customer = new Customer()
        {
            Login = model.Login,
            Mail = model.Mail,
            Name = model.Name,
            Surname = model.Surname,
            Phone = model.Phone,
            Carts = new List<Carts>(),
            Orders = new List<Orders>()
        };

        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();

        return customer.Id;
    }
}
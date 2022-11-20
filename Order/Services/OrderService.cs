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
using Order.Exceptions;

namespace Order.Services;
public class OrderService : IOrderService
{
    private OrderDbContext _context { get; set; }

    public OrderService(OrderDbContext context) {
        _context = context;
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
            Items = new List<ItemModel>()
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
                IsOnDelivery = order.IsOnDelivery
            });
        }

        return res;
    }

    public async Task<OrderModel> PostOrder(OrderModel model, int customerId)
    {
        if(customerId < 0)
            throw new ArgumentException("Id must be >= 0");

        if (model.Items is null)
            throw new InvalidOrderException("Order can't be empty");

        var customer = await _context.Customers.FindAsync(customerId);
        if(customer is null)
            throw new InvalidCustomerException("couldn't find customer with such Id");

        //TODO Check Items

        var items = new List<OrderItem>();

        foreach (var item in model.Items)
        {
            items.Add(new OrderItem() {
                Name = item.Name,
                Article = item.Article,
                Descr = item.Descr,
                Brand = item.Brand,
                Category = item.Category,
                Count = item.Count
            });
        }

        await _context.Orders.AddAsync(new Orders() {
            Customer = customer,
            CustomerId = customer.Id,
            Items = items,
            IsCanceled = model.IsCanceled,
            IsOnDelivery = model.IsOnDelivery,
            IsFinished = model.IsFinished
        });

        return model;
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

        var itemmodels = new List<ItemModel>();
        foreach(var prod in cart.Items) {
            itemmodels.Add(new ItemModel() {
                Name = prod.Name,
                Article = prod.Article,
                Descr = prod.Descr,
                Brand = prod.Brand,
                Category = prod.Category,
                Count = prod.Count
            });
        }

        return new CartModel() {
            Id = cart.Id,
            Items = itemmodels
        };
    }

    public async Task CancelOrder(int orderId)
    {
        if (orderId < 0)
            throw new ArgumentException("OrderId should be >= 0");

        var order = await _context.Orders.SingleOrDefaultAsync(x => x.Id == orderId);
        if (order is null)
            throw new InvalidOrderException("No such order");
        if (order.IsCanceled == true)
            throw new AlreadyCanceledException("This order is canceled already");
        
        //TODO /api/v1/Delivery/CancelOrder

        order.IsCanceled = true;

        await _context.SaveChangesAsync();

        return;
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
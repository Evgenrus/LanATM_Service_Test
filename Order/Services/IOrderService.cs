using System;
using Order.Models;

namespace Order.Services;
public interface IOrderService
{
    public Task<CartModel> GetCartById(int id);
    public Task<ICollection<CartModel>> GetCartsByCustomerId(int id);
    public Task<CustomerModel> GetCustomerByLogin(string Login);
    public Task<OrderModel> GetOrderById(int id);
    public Task<ICollection<OrderModel>> GetOrdersByCustomerId(int id);
    public Task<OrderModel> PostOrder(PostOrderModel model);
    public Task<CartModel> NewCart(int customerId);
    public Task<CartModel> AddItemToCart(ItemModel model, int cartId, int customerId);
    public Task CancelOrder(int orderId);
    public Task<int> PostNewCustomer(CustomerModel model);
}
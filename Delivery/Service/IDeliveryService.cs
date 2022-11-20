using Delivery.Database.Entities;
using Delivery.Models;

namespace Delivery.Service;

public interface IDeliveryService
{
    public Task PostNewDelivery(DeliveryModel delivery);
    public Task<ICollection<DeliveryModel>> GetAllFreeOrders();
    public Task BeginOrderDelivery(int orderId, int courierId);
    public Task ReceiveOrderFromStock(int orderId, int courierId);
    public Task FinishDelivery(int orderId, int courierId);
    public Task<string> DeliveryStatus(int orderId);
    public Task<ICollection<AddressModel>> GetAddressesByCustomerId(int id);
    public Task<int> AddNewCustomer(CustomerModel model);
    public Task<int> AddNewCourier(CourierModel model);
    public Task<int> PostNewAddress(AddressModel model);
}
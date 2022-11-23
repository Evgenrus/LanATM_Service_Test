using Infrastructure.Models;

namespace Catalog.Services;

public interface IRabbitmqService
{
    public Task<RestockRespList> OrderItems(RestockRequestList items);
    public Task<RestockResp> Restock(RestockRequest request);
}
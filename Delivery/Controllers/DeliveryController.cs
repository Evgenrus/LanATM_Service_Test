using Delivery.Models;
using Delivery.Service;
using Infrastructure.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class DeliveryController : ControllerBase
{
    private readonly ILogger<DeliveryController> _logger;
    private readonly IDeliveryService _service;
    private IRequestClient<ItemsToCheckList> _request;

    public DeliveryController(ILogger<DeliveryController> logger, IDeliveryService service, IRequestClient<ItemsToCheckList> request)
    {
        _logger = logger;
        _service = service;
        _request = request;
    }

    // [HttpPost]
    // public async Task<ActionResult> PostNewDelivery(DeliveryModel delivery)
    // {
    //     if (!ModelState.IsValid)
    //         return BadRequest();
    //     try
    //     {
    //         await _service.PostNewDelivery(delivery);
    //         return NoContent();
    //     }
    //     catch (Exception e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    // }

    [HttpGet]
    public async Task<ActionResult<ICollection<DeliveryModel>>> GetAllFreeOrders()
    {
        try
        {
            var res = await _service.GetAllFreeOrders();
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPatch]
    public async Task<ActionResult> BeginOrderDelivery(int orderId, int courierId)
    {
        try
        {
            await _service.BeginOrderDelivery(orderId, courierId);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPatch]
    public async Task<ActionResult> ReceiveOrderFromStock(int orderId, int courierId)
    {
        try
        {
            await _service.ReceiveOrderFromStock(orderId, courierId);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPatch]
    public async Task<ActionResult> FinishDelivery(int orderId, int courierId)
    {
        try
        {
            await _service.FinishDelivery(orderId, courierId);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<string>> DeliveryStatus(int orderId)
    {
        try
        {
            var res = await _service.DeliveryStatus(orderId);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<AddressModel>>> GetAddressesByCustomerId(int id)
    {
        try
        {
            var res = await _service.GetAddressesByCustomerId(id);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddNewCustomer(CustomerModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        try
        {
            var res = await _service.AddNewCustomer(model);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddNewCourier(CourierModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        try
        {
            var res = await _service.AddNewCourier(model);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> PostNewAddress(AddressModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        try
        {
            var res = await _service.PostNewAddress(model);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
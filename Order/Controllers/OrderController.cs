using Microsoft.AspNetCore.Mvc;
using Order.Models;
using Order.Services;
using System;

namespace Order.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class OrderController : ControllerBase
{

    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _service;

    public OrderController(ILogger<OrderController> logger, IOrderService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<CartModel>> GetCartById(int id) {
        try
        {
            var res = await _service.GetCartById(id);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<CartModel>>> GetCartsByCustomerId(int id) {
        try
        {
            var res = await _service.GetCartsByCustomerId(id);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<CustomerModel>> GetCustomerIdByLogin(string login) {
        try
        {
            var res = await _service.GetCustomerByLogin(login);
            return res;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<OrderModel>> GetOrderById(int id) {
        try
        {
            var res = await _service.GetOrderById(id);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<OrderModel>>> GetOrdersByCustomerId(int id) {
        try
        {
            var res = await _service.GetOrdersByCustomerId(id);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    } 

    [HttpPost]
    public async Task<ActionResult<OrderModel>> PostOrder(PostOrderModel model, int customerId) {
        if(!ModelState.IsValid)
            return BadRequest();

        try
        {
            var res = await _service.PostOrder(model);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    } 

    [HttpPost]
    public async Task<ActionResult<CartModel>> NewCart(int customerId) {
        if(!ModelState.IsValid)
            return BadRequest();

        try
        {
            var res = await _service.NewCart(customerId);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<CartModel>> AddItemToCart(ItemModel model, int cartId, int customerId) {
        if (!ModelState.IsValid)
            return BadRequest();

        try
        {
            var res = await _service.AddItemToCart(model, cartId, customerId);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch]
    public async Task<ActionResult> CancelOrder(int orderId) {
        try
        {
            await _service.CancelOrder(orderId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> PostNewCustomer(CustomerModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        try
        {
            var res = await _service.PostNewCustomer(model);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
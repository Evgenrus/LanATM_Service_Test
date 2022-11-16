using Catalog.Models;
using Catalog.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class CatalogController : ControllerBase
{
    private readonly ILogger<CatalogController> _logger;
    private readonly ICatalogService _service;

    public CatalogController(ILogger<CatalogController> logger, ICatalogService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ItemModel>> GetItemById(int id)
    {
        try
        {
            var res = await _service.GetItemById(id);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<ItemModel>>> GetItemsByBrandName(string brand)
    {
        try
        {
            var res = await _service.GetItemsByBrandName(brand);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<ItemModel>> GetItemByName(string name)
    {
        try
        {
            var res = await _service.GetItemByName(name);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<ItemModel>>> GetItemsByCategoryName(string category)
    {
        try
        {
            var res = await _service.GetItemsByCategoryName(category);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> PostNewItem(ItemModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        
        try
        {
            await _service.PostNewItem(model);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> PostNewBrand(BrandModel model)
    {
        if(!ModelState.IsValid)
            return BadRequest();

        try
        {
            await _service.PostNewBrand(model);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> PostNewCategory(CategoryModel model)
    {
        return StatusCode(501);
    }

    [HttpPatch]
    public async Task<ActionResult> Restock(string article, int count)
    {
        return StatusCode(501);
    }

    [HttpPost]
    public async Task<ActionResult<ItemModel>> AddItemToCart(ItemModel model)
    {
        return StatusCode(501);
    }

    [HttpPost]
    public async Task<ActionResult<ICollection<ItemModel>>> OrderItems(List<ItemModel> items)
    {
        return StatusCode(501);
    }
}
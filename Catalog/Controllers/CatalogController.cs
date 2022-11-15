using Catalog.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class CatalogController : ControllerBase
{
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(ILogger<CatalogController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ItemModel>> GetItemById(int id)
    {
        return StatusCode(501);
    }

    [HttpGet]
    public async Task<ActionResult<List<ItemModel>>> GetItemsByBrandName(string brand)
    {
        return StatusCode(501);
    }

    [HttpGet]
    public async Task<ActionResult<ItemModel>> GetItemByName(string name)
    {
        return StatusCode(501);
    }

    [HttpGet]
    public async Task<ActionResult<List<ItemModel>>> GetItemsByCategoryName(string category)
    {
        return StatusCode(501);
    }

    [HttpPost]
    public async Task<ActionResult> PostNewItem(ItemModel model)
    {
        return StatusCode(501);
    }
}
using System.ComponentModel.DataAnnotations;

namespace Catalog.Database.Entities;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Article { get; set; }
    public string Descr { get; set; }
    public int BrandId { get; set; }
    public Brand Brand { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public int Stock { get; set; }
}
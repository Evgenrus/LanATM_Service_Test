namespace Catalog.Models;

public class ItemModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Article { get; set; }
    public string Descr { get; set; }
    public string Brand { get; set; }
    public string Category { get; set; }
    public int Stock { get; set; }
}
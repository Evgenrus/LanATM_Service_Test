namespace Catalog.Database.Entities;

public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Descr { get; set; }
    public string LogoURL { get; set; }

    public List<Item> Items { get; set; } = new List<Item>();
}
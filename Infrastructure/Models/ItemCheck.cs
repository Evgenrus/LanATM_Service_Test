namespace Infrastructure.Models;

public class ItemCheck
{
    public bool IsCorrect { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Article { get; set; }
    public string Brand { get; set; }
    public string Category { get; set; }
    public int RequestingCount { get; set; }
    public int Stock { get; set; } 
}
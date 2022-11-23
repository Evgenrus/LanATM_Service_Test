namespace Infrastructure.Models;

public interface IDeliveryCancelAnswer
{
    public int Id { get; set; }
    public string? ErrMsg { get; set; }
}

public class DeliveryCancelAnswer : IDeliveryCancelAnswer
{
    public int Id { get; set; }
    public string? ErrMsg { get; set; }
}
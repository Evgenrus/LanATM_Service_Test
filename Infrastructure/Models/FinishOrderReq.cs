namespace Infrastructure.Models;

public interface IFinishOrderRequest
{
    public int Id { get; set; }
}

public class FinishOrderRequest : IFinishOrderRequest
{
    public int Id { get; set; }
}
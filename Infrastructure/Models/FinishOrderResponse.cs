namespace Infrastructure.Models;

public interface IFinishOrderResponse
{
    public int Id { get; set; }
    public string ErrMsg { get; set; }
}

public class FinishOrderResponse : IFinishOrderResponse
{
    public int Id { get; set; }
    public string ErrMsg { get; set; }
}
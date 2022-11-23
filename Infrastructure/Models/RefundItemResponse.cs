namespace Infrastructure.Models;

public interface IRefundItemResponse
{
    public int Stock { get; set; }
    public int Requested { get; set; }
    public string ErrMsg { get; set; }
}

public class RefundItemResponse : IRefundItemResponse
{
    public int Stock { get; set; }
    public int Requested { get; set; }
    public string ErrMsg { get; set; }
}
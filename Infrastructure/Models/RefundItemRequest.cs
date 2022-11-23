namespace Infrastructure.Models;

public interface IRefundItemRequest
{
    public string Article { get; set; }
    public int Change { get; set; }
}

public class RefundItemRequest : IRefundItemRequest
{
    public string Article { get; set; }
    public int Change { get; set; }
}
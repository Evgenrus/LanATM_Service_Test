namespace Infrastructure.Models;

public interface IRestockRequest
{
    public string Article { get; set; }
    public int Change { get; set; }
}

public class RestockRequest : IRestockRequest
{
    public string Article { get; set; }
    public int Change { get; set; }
}

public interface IRestockRequestList
{
    public List<RestockRequest> Items { get; set; }
}

public class RestockRequestList : IRestockRequestList
{
    public List<RestockRequest> Items { get; set; }
}
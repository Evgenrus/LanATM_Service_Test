using MassTransit;
using MassTransit.Futures.Contracts;

namespace Infrastructure.Models;

public interface IRestockResp
{
    public int Stock { get; set; }
    public int Requested { get; set; }
    public string ErrMsg { get; set; }
}

public class RestockResp : IRestockResp
{
    public int Stock { get; set; }
    public int Requested { get; set; }
    public string ErrMsg { get; set; }
}

public interface IRestockRespList
{
    public List<RestockResp> Items { get; set; }
}

public class RestockRespList : IRestockRespList
{
    public List<RestockResp> Items { get; set; }
}

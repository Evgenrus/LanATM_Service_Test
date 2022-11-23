using MassTransit;

namespace Infrastructure.Helpers;

public class RabbitMQHelpers
{
    public static async Task<TOut> GetResponseRabbitTask<TIn, TOut>(IBusControl bus, TIn request, Uri uri)
        where TIn : class
        where TOut : class
    {
        var client = bus.CreateRequestClient<TIn>(uri);
        var res = await client.GetResponse<TOut>(request);
        return res.Message;
    }
}
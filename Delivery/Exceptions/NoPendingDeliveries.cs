namespace Delivery.Exceptions;

public class NoPendingDeliveries : Exception
{
    public NoPendingDeliveries(string msg)
        : base(msg)
    { }
}
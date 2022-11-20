namespace Delivery.Exceptions;

public class EmptyDeliveryException : Exception
{
    public EmptyDeliveryException(string msg)
        : base(msg)
    { }
}
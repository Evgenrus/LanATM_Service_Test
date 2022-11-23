namespace Delivery.Exceptions;

public class InvalidDeliveryException : Exception
{
    public InvalidDeliveryException(string msg)
        : base(msg)
    { }
}
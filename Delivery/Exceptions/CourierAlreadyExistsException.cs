namespace Delivery.Exceptions;

public class CourierAlreadyExistsException : Exception
{
    public CourierAlreadyExistsException(string msg)
        : base(msg)
    { }
}
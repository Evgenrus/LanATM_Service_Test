namespace Delivery.Exceptions;

public class CustomerAlreadyExistsException : Exception
{
    public CustomerAlreadyExistsException(string msg)
        : base(msg)
    { }
}
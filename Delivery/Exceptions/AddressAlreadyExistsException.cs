namespace Delivery.Exceptions;

public class AddressAlreadyExists : Exception
{
    public AddressAlreadyExists(string msg)
        : base(msg)
    { }
}
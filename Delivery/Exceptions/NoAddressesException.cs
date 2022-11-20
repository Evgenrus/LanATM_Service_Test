namespace Delivery.Exceptions;

public class NoAddressesException : Exception
{
    public NoAddressesException(string msg)
        : base(msg)
    { }
}
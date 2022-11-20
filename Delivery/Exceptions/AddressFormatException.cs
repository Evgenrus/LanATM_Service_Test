namespace Delivery.Exceptions;

public class AddressFormatException : Exception
{
    public AddressFormatException(string msg)
        : base(msg)
    { }
}
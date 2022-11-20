namespace Delivery.Exceptions;

public class InvalidAddressException : Exception
{
    public InvalidAddressException(string msg) 
        : base(msg)
    { }
}
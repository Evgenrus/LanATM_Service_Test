namespace Catalog.Exceptions;

public class InvalidItemException : Exception
{
    public InvalidItemException(string msg)
        : base(msg)
    { }
}
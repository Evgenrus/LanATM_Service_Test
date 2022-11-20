namespace Catalog.Exceptions;

public class InvalidBrandException : Exception
{
    public InvalidBrandException(string msg)
        :base(msg)
    { }
}
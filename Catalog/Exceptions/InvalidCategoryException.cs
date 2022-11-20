namespace Catalog.Exceptions;

public class InvalidCategoryException : Exception
{
    public InvalidCategoryException(string msg)
        :base(msg)
    { }
}
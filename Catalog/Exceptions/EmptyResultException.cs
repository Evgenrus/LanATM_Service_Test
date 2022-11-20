namespace Catalog.Exceptions;

public class EmptyResultException : Exception
{
    public EmptyResultException(String msg)
        : base(msg) 
    { }
}
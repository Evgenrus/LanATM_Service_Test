namespace Catalog.Exceptions;

public class EmptyItemsListException : Exception
{
    public EmptyItemsListException(string msg)
        : base(msg) 
    { }
}
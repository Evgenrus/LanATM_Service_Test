namespace Catalog.Exceptions;

public class IncorrectIdException : Exception
{
    public IncorrectIdException(string message)
        :base(message)
    { }
}
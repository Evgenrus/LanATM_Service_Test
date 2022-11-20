namespace Order.Exceptions;

public class InvalidCartException : Exception
{
    public InvalidCartException(string msg)
        : base(msg) 
        { } 
}
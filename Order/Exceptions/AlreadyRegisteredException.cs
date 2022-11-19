namespace Order.Exceptions;

public class AlreadyRegisteredException : Exception
{
    public AlreadyRegisteredException(string msg)
        : base(msg)
    { }
}
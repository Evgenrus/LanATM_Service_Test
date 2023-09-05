namespace Order.Exceptions;

public class PostDeliveryFailedException : Exception
{
    public PostDeliveryFailedException(string msg)
        : base(msg)
    { }
}
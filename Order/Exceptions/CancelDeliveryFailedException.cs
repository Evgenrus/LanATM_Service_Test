namespace Order.Exceptions;

public class CancelDeliveryFailedException : Exception
{
    public CancelDeliveryFailedException(string msg)
        : base(msg)
    { }
}
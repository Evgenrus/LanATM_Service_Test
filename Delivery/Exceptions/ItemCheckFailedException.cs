namespace Delivery.Exceptions;

public class ItemCheckFailedException : Exception
{
    public ItemCheckFailedException(string msg)
        : base(msg) 
    { }
}
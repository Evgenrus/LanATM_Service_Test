namespace Order.Exceptions;

public class InvalidCustomerException : Exception {
    public InvalidCustomerException(string msg)
        : base(msg)
    { }
}
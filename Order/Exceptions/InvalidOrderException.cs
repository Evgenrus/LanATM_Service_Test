namespace Order.Exceptions;

public class InvalidOrderException : Exception {
    public InvalidOrderException(string msg) 
        : base(msg) 
    { }
}
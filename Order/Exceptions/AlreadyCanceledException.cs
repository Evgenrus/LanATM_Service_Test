namespace Order.Exceptions;

public class AlreadyCanceledException : Exception {
    public AlreadyCanceledException(string msg)
        : base (msg) 
    { }
}
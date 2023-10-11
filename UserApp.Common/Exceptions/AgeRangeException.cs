namespace UserApp.Common.Exceptions;

public class AgeRangeException : Exception
{
    public AgeRangeException(string message) : base(message)
    {
    }
}
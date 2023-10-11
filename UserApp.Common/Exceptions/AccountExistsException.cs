namespace UserApp.Common.Exceptions;

public class AccountExistsException : Exception
{
    public AccountExistsException(string message) : base(message)
    {
    }
}
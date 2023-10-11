namespace UserApp.Common.Extensions;

public class AccountExistsException : Exception
{
    public AccountExistsException(string message) : base(message)
    {
    }
}
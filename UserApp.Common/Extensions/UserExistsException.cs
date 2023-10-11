namespace UserApp.Common.Extensions;

public class UserExistsException : Exception
{
    public UserExistsException(string message) : base(message)
    {
    }
}
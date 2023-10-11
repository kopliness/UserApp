namespace UserApp.Common.Extensions;

public class IncorrectRolesException : Exception
{
    public IncorrectRolesException(string message) : base(message)
    {
    }
}
namespace UserApp.Common.Exceptions;

public class IncorrectRolesException : Exception
{
    public IncorrectRolesException(string message) : base(message)
    {
    }
}
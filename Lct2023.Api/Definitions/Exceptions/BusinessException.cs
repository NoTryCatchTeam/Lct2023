namespace Lct2023.Api.Definitions.Exceptions;

public class BusinessException : Exception
{
    public BusinessException(string message)
        : base(message)
    {
    }
}

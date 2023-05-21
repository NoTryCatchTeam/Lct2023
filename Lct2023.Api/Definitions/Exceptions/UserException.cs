using Lct2023.Api.Definitions.Types;

namespace Lct2023.Api.Definitions.Exceptions;

public class UserException : TypedException<UserExceptionType>
{
    public UserException(UserExceptionType type)
        : base(type)
    {
    }

    public UserException(string message, UserExceptionType type)
        : base(message, type)
    {
    }
}

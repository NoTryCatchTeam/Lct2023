namespace Lct2023.Api.Definitions.Exceptions;

public class OauthException : Exception
{
    // TODO Inherit TypedException if needed
    public OauthException(string message)
        : base(message)
    {
    }
}

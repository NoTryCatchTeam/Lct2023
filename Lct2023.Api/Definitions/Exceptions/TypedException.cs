namespace Lct2023.Api.Definitions.Exceptions;

public abstract class TypedException<TEnum> : Exception
    where TEnum : Enum
{
    protected TypedException(TEnum type)
    {
        Type = type;
    }

    protected TypedException(string message, TEnum type)
        : base(message)
    {
        Type = type;
    }

    public TEnum Type { get; }
}

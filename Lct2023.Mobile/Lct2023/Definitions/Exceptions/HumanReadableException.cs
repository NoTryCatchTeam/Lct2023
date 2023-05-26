using System;

namespace Lct2023.Definitions.Exceptions;

public class HumanReadableException : Exception
{
    public HumanReadableException(string message)
        : base(message)
    {
    }

    public HumanReadableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

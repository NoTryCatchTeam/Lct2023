using System;

namespace Lct2023.Definitions.Types;

[Flags]
public enum AuthViewState
{
    SigningIn = 1 << 0,

    SigningInViaVk = 1 << 1,

    SigningUp = 1 << 2,
}
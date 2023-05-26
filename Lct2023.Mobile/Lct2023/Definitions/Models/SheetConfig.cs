using System.Collections.Generic;

namespace Lct2023.Definitions.Models;

public class SheetConfig
{
    public SheetConfig(IEnumerable<string> buttons, string cancelButton)
    {
        Buttons = buttons;
        CancelButton = cancelButton;
    }

    public SheetConfig(string title, IEnumerable<string> buttons, string cancelButton)
        : this(buttons, cancelButton)
    {
        Title = title;
    }

    public string Title { get; }

    public IEnumerable<string> Buttons { get; }

    public string CancelButton { get; }
}

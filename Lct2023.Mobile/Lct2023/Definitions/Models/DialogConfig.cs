namespace Lct2023.Definitions.Models;

public class DialogConfig
{
    public DialogConfig(string title, string message, string positiveButton)
    {
        Title = title;
        Message = message;
        PositiveButton = positiveButton;
    }

    public DialogConfig(string title, string message, string positiveButton, string negativeButton)
        : this(title, message, positiveButton)
    {
        NegativeButton = negativeButton;
    }

    public string Title { get; }

    public string Message { get; }

    public string PositiveButton { get; }

    public string NegativeButton { get; }
}

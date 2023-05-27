using Google.Android.Material.AppBar;

namespace Lct2023.Android.Helpers;

public class InnerToolbar : BaseAppToolbar
{
    public InnerToolbar(MaterialToolbar toolbar)
        : base(toolbar)
    {
    }

    public override string Title
    {
        get => Toolbar.Title;
        set => Toolbar.Title = value;
    }
}
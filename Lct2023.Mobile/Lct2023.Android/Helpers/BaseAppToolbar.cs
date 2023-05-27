using Google.Android.Material.AppBar;

namespace Lct2023.Android.Helpers;

public abstract class BaseAppToolbar
{
    protected BaseAppToolbar(MaterialToolbar toolbar)
    {
        Toolbar = toolbar;
    }

    public MaterialToolbar Toolbar { get; }

    public abstract string Title { get; set; }
}
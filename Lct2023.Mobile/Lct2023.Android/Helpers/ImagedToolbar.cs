using Android.Widget;
using Google.Android.Material.AppBar;

namespace Lct2023.Android.Helpers;

public class ImagedToolbar : BaseAppToolbar
{
    private readonly TextView _titleView;

    public ImagedToolbar(MaterialToolbar toolbar)
        : base(toolbar)
    {
        Avatar = toolbar.FindViewById<ImageView>(Resource.Id.toolbar_image);
        _titleView = toolbar.FindViewById<TextView>(Resource.Id.toolbar_title);
    }

    public ImageView Avatar { get; }

    public override string Title
    {
        get => _titleView.Text;
        set => _titleView.Text = value;
    }
}
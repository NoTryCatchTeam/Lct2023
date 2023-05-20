using Android.App;
using Android.Content.PM;
using Android.OS;
using Lct2023.Android.Views;
using Lct2023.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;

namespace Lct2023.Android.Activities;

[MvxActivityPresentation]
[Activity]
public abstract class BaseActivity<TViewModel> : MvxActivity<TViewModel>
    where TViewModel : BaseViewModel
{
    private ExtendedToolbar _toolbar;

    protected ExtendedToolbar Toolbar
    {
        get => _toolbar;
        set
        {
            _toolbar = value;

            if (value != null)
            {
                var set = CreateBindingSet();

                set.Bind(value)
                    .For(x => x.NavigationIconClickCommand)
                    .To(vm => vm.NavigateBackCommand);

                set.Apply();
            }
        }
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}
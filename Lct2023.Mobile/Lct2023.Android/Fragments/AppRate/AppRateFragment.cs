using System.Linq;
using Android.OS;
using Android.Views;
using DynamicData;
using Google.Android.Material.Button;
using Lct2023.Android.Listeners;
using Lct2023.ViewModels.AppRate;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace Lct2023.Android.Fragments.AppRate;

[MvxDialogFragmentPresentation(Cancelable = true)]
public class AppRateFragment : MvxDialogFragment<AppRateViewModel>
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        base.OnCreateView(inflater, container, savedInstanceState);

        var view = this.BindingInflate(Resource.Layout.AppRateFragment, container, false);

        var stars = new[]
        {
            view.FindViewById<MaterialButton>(Resource.Id.app_rate_star1),
            view.FindViewById<MaterialButton>(Resource.Id.app_rate_star2),
            view.FindViewById<MaterialButton>(Resource.Id.app_rate_star3),
            view.FindViewById<MaterialButton>(Resource.Id.app_rate_star4),
            view.FindViewById<MaterialButton>(Resource.Id.app_rate_star5),
        };

        var send = view.FindViewById<MaterialButton>(Resource.Id.app_rate_send);
        var close = view.FindViewById<MaterialButton>(Resource.Id.app_rate_close);

        var clickListener = new DefaultClickListener(v =>
        {
            var tappedStarIndex = stars.IndexOf(v);

            for (var i = 0; i < stars.Length; i++)
            {
                var star = stars[i];

                star.SetIconResource(i <= tappedStarIndex ? Resource.Drawable.ic_star : Resource.Drawable.ic_star_outline);
            }
        });

        foreach (var star in stars)
        {
            star.SetOnClickListener(clickListener);
        }

        var set = CreateBindingSet();

        set.Bind(send)
            .For(x => x.BindClick())
            .To(vm => vm.SendCommand);

        set.Bind(close)
            .For(x => x.BindClick())
            .To(vm => vm.NavigateBackCommand);

        set.Apply();

        return view;
    }

    public override void OnStart()
    {
        base.OnStart();

        Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
    }
}

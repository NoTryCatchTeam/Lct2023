using Android.App;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager2.Widget;
using Lct2023.Android.Helpers;

namespace Lct2023.Android.Callbacks;

public class AuthViewPagerPageCallback : ViewPager2.OnPageChangeCallback
{
    private readonly ViewPager2 _viewPager;
    private readonly View _viewPagerParent;
    private readonly DefaultBackPressedCallback _activityBackPressedCallback;
    private readonly RecyclerView _recyclerView;

    public AuthViewPagerPageCallback(ViewPager2 viewPager, View viewPagerParent, DefaultBackPressedCallback activityBackPressedCallback)
    {
        _viewPager = viewPager;
        _viewPagerParent = viewPagerParent;
        _activityBackPressedCallback = activityBackPressedCallback;
        _recyclerView = viewPager.GetChildAt(0) as RecyclerView;
    }

    public override void OnPageSelected(int position)
    {
        base.OnPageSelected(position);

        _activityBackPressedCallback.Enabled = position > 0;

        (_viewPager.Context as Activity)?.HideKeyboard();

        if (_recyclerView?.GetLayoutManager()?.FindViewByPosition(position) is not { } view)
        {
            return;
        }

        var wMeasuredSpec = View.MeasureSpec.MakeMeasureSpec(view.Width, MeasureSpecMode.Exactly);
        var hMeasuredSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);

        view.Measure(wMeasuredSpec, hMeasuredSpec);

        var layoutParameters = _viewPager.LayoutParameters;

        if (view.MeasuredHeight != layoutParameters.Height)
        {
            layoutParameters.Height = view.MeasuredHeight + _viewPager.PaddingTop;
            _viewPager.LayoutParameters = layoutParameters;

            _viewPagerParent.RequestLayout();
        }
    }
}

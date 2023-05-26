using Android.Animation;
using Android.Graphics;
using Android.Views;
using AndroidX.Core.Widget;
using Lct2023.Android.Listeners;

namespace Lct2023.Android.Helpers;

public class ScrollWithSearchLayoutMediator
{
    private (Animator Show, Animator Hide) _scrollAnimators;

    public ScrollWithSearchLayoutMediator(ViewGroup parent, NestedScrollView scroll, View searchLayout)
    {
        parent.Post(() =>
        {
            var searchRect = new Rect();
            searchLayout.GetDrawingRect(searchRect);
            parent.OffsetDescendantRectToMyCoords(searchLayout, searchRect);

            scroll.SetPadding(scroll.PaddingLeft, scroll.PaddingTop + searchRect.Height(), scroll.PaddingRight, scroll.PaddingBottom);

            _scrollAnimators = (
                ObjectAnimator.OfFloat(searchLayout, nameof(View.TranslationY), -searchRect.Height(), 0)
                    .SetDuration(200),
                ObjectAnimator.OfFloat(searchLayout, nameof(View.TranslationY), 0, -searchRect.Height())
                    .SetDuration(200));
        });

        scroll.SetOnScrollChangeListener(new DefaultScrollListener((_, _, scrollY, _, oldScrollY) =>
        {
            const int SCROLL_DIFF_THRESHOLD = 3;

            var isScrollDown = scrollY - oldScrollY > 0;
            var isScrollUp = scrollY - oldScrollY < 0;

            if (isScrollDown && scrollY - oldScrollY > SCROLL_DIFF_THRESHOLD && !_scrollAnimators.Hide.IsRunning && searchLayout.TranslationY == 0)
            {
                _scrollAnimators.Show.Cancel();
                _scrollAnimators.Hide.Start();
            }
            else if (isScrollUp && (scrollY - oldScrollY < -SCROLL_DIFF_THRESHOLD || scrollY < scroll.PaddingTop / 2) && !_scrollAnimators.Show.IsRunning && searchLayout.TranslationY < 0)
            {
                _scrollAnimators.Hide.Cancel();
                _scrollAnimators.Show.Start();
            }
        }));
    }
}

using Android.Animation;
using Android.Graphics;
using Android.Views;
using AndroidX.Core.Widget;
using Lct2023.Android.Listeners;

namespace Lct2023.Android.Helpers;

public class ScrollWithOverlayViewMediator
{
    private (Animator Show, Animator Hide) _scrollAnimators;

    public ScrollWithOverlayViewMediator(ViewGroup parent, NestedScrollView scroll, View overlayView)
    {
        parent.Post(() =>
        {
            var overlayViewRect = new Rect();
            overlayView.GetDrawingRect(overlayViewRect);
            parent.OffsetDescendantRectToMyCoords(overlayView, overlayViewRect);

            scroll.SetPadding(scroll.PaddingLeft, scroll.PaddingTop + overlayViewRect.Height(), scroll.PaddingRight, scroll.PaddingBottom);

            _scrollAnimators = (
                ObjectAnimator.OfFloat(overlayView, nameof(View.TranslationY), -overlayViewRect.Height(), 0)
                    .SetDuration(200),
                ObjectAnimator.OfFloat(overlayView, nameof(View.TranslationY), 0, -overlayViewRect.Height())
                    .SetDuration(200));
        });

        scroll.SetOnScrollChangeListener(new DefaultScrollListener((_, _, scrollY, _, oldScrollY) =>
        {
            const int SCROLL_DIFF_THRESHOLD = 3;

            var isScrollDown = scrollY - oldScrollY > 0;
            var isScrollUp = scrollY - oldScrollY < 0;

            if (isScrollDown && scrollY - oldScrollY > SCROLL_DIFF_THRESHOLD && !_scrollAnimators.Hide.IsRunning && overlayView.TranslationY == 0)
            {
                _scrollAnimators.Show.Cancel();
                _scrollAnimators.Hide.Start();
            }
            else if (isScrollUp && (scrollY - oldScrollY < -SCROLL_DIFF_THRESHOLD || scrollY < scroll.PaddingTop / 2) && !_scrollAnimators.Show.IsRunning && overlayView.TranslationY < 0)
            {
                _scrollAnimators.Hide.Cancel();
                _scrollAnimators.Show.Start();
            }
        }));
    }
}

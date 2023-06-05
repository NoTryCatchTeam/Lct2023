using Android.Content;
using Android.Util;
using Android.Views;
using AndroidX.Preference;
using R = Android.Resource;

namespace Lct2023.Android.Views
{
    public class DimView : View
    {
        public DimView(Context context)
        : base(context)
        {
            SetBackgroundResource(R.Color.Black);
            Alpha = 0f;
        }

        public DimView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            SetBackgroundResource(R.Color.Black);
            Alpha = 0f;
        }

        public override bool OnTouchEvent(MotionEvent @event) => false;
    }
}


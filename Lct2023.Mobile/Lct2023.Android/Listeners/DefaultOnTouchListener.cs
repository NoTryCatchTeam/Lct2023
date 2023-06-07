using System;
using Android.Views;

namespace Lct2023.Android.Listeners
{
	public class DefaultOnTouchListener : Java.Lang.Object, View.IOnTouchListener
    {
        private Func<View, MotionEvent, bool> _touchAction;

        public DefaultOnTouchListener(Func<View, MotionEvent, bool> touchAction)
		{
            _touchAction = touchAction;

        }

        public bool OnTouch(View view, MotionEvent @event) => _touchAction(view, @event);
    }
}


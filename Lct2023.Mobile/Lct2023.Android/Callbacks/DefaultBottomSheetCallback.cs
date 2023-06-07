using System;
using Android.Views;
using static Google.Android.Material.BottomSheet.BottomSheetBehavior;

namespace Lct2023.Android.Callbacks
{
	public class DefaultBottomSheetCallback : BottomSheetCallback
    {
        private Action<View, float> _onSlideAction;
        private Action<View, int> _onStateChangedAction;

        public DefaultBottomSheetCallback(Action<View, float> onSlideAction = null, Action<View, int> onStateChangedAction = null)
		{
            _onSlideAction = onSlideAction;
            _onStateChangedAction = onStateChangedAction;
        }

        public override void OnSlide(View bottomSheet, float newState) => _onSlideAction?.Invoke(bottomSheet, newState);

        public override void OnStateChanged(View p0, int p1) => _onStateChangedAction?.Invoke(p0, p1);
    }
}


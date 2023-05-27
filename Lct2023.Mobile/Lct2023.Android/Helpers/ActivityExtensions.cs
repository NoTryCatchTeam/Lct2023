using Android.App;
using Android.Content;
using Android.Views.InputMethods;

namespace Lct2023.Android.Helpers;

public static class ActivityExtensions
{
    public static void HideKeyboard(this Activity activity)
    {
        if (activity.CurrentFocus is not { } focusedView)
        {
            return;
        }

        focusedView.ClearFocus();
        ((InputMethodManager)activity.GetSystemService(Context.InputMethodService)).HideSoftInputFromWindow(focusedView.WindowToken, HideSoftInputFlags.None);
    }
}

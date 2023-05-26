using Android.App;
using Android.Content;
using Android.Views.InputMethods;

namespace Lct2023.Android.Helpers;

public static class KeyboardUtils
{
    public static void HideKeyboard(Activity activity)
    {
        var view = activity.CurrentFocus;
        if (view == null)
        {
            return;
        }
        
        (activity.GetSystemService(Context.InputMethodService) as InputMethodManager)?.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.None);

    }
}
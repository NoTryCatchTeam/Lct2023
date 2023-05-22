using Android.Content;

namespace Lct2023.Android.Helpers;

public static class DimensUtils
{
    public static int PxToDp(Context context, int px) =>
        (int)(px / context.Resources.DisplayMetrics.Density);

    public static int DpToPx(Context context, int dp) =>
        (int)(dp * context.Resources.DisplayMetrics.Density);
}

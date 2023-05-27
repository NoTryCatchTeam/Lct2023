using Android.Graphics;
using Android.Graphics.Drawables;

namespace Lct2023.Android.Helpers;

public class PinUtils
{
    public static Bitmap CreateBitmap(int diameter, params Drawable[] drawables)
    {
        var bitmap = Bitmap.CreateBitmap(diameter, diameter, Bitmap.Config.Argb8888);
        var canvas = new Canvas(bitmap);
        foreach (var drawable in drawables)
        {
            drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
            drawable.Draw(canvas);
        }
        return bitmap;
    }
}
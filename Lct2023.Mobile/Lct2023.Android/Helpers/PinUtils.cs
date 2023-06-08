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

    public static Bitmap CreateBitmapWithText(int diameter, string text, Color textColor, float textSize, params Drawable[] drawables)
    {
        var bitmap = Bitmap.CreateBitmap(diameter, diameter, Bitmap.Config.Argb8888);
        var canvas = new Canvas(bitmap);
        foreach (var drawable in drawables)
        {
            drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
            drawable.Draw(canvas);
        }

        var paint = new Paint
        {
            TextSize = textSize,
            TextAlign = Paint.Align.Center,
            Color = textColor
        };

        var textBounds = new Rect();
        paint.GetTextBounds(text, 0, text.Length, textBounds);
        canvas.DrawText(text, diameter / 2f, diameter / 2f + textBounds.Height() / 2f, paint);

        return bitmap;
    }
}
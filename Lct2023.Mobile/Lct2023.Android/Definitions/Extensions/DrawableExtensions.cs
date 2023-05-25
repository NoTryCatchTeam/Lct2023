using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using AndroidX.Core.Content;

namespace Lct2023.Android.Definitions.Extensions;

public static class DrawableExtensions
{
    public static Drawable GetDrawable(this int resId, Context context) => ContextCompat.GetDrawable(context, resId);

    public static Bitmap AddCircleWithStroke(this Drawable drawable, int strokeWidth, Color strokeColor, int colorId, Context context)
    {
        var diameter = Math.Min(drawable.IntrinsicWidth, drawable.IntrinsicHeight);
        var circularDrawable = new GradientDrawable();
        circularDrawable.SetShape(ShapeType.Oval);
        circularDrawable.SetSize(diameter, diameter);
        circularDrawable.SetColor(ContextCompat.GetColor(context, colorId));
        circularDrawable.SetStroke(strokeWidth, strokeColor);
        var bitmap = Bitmap.CreateBitmap(diameter, diameter, Bitmap.Config.Argb8888);
        var canvas = new Canvas(bitmap);
        drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
        circularDrawable.SetBounds(0, 0, canvas.Width, canvas.Height);
        circularDrawable.Draw(canvas);
        drawable.Draw(canvas);
        return bitmap;
    }
}
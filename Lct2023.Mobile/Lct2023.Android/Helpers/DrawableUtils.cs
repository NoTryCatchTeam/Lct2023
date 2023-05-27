using Android.Graphics;
using Android.Graphics.Drawables;

namespace Lct2023.Android.Helpers;

public class DrawableUtils
{
    public static Drawable CreateCircleDrawable(int diameter, int strokeWidth, Color strokeColor, Color solidColor)
    {
        var circularDrawable = new GradientDrawable();
        circularDrawable.SetShape(ShapeType.Oval);
        circularDrawable.SetSize(diameter, diameter);
        circularDrawable.SetColor(solidColor);
        circularDrawable.SetStroke(strokeWidth, strokeColor);
        return circularDrawable;
    }
}
using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using AndroidX.Core.Content;

namespace Lct2023.Android.Definitions.Extensions;

public static class DrawableExtensions
{
    public static Drawable GetDrawable(this int resId, Context context) => ContextCompat.GetDrawable(context, resId);
}
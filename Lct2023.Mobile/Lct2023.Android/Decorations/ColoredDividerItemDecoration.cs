using System;
using Android.Content;
using Android.Graphics;
using AndroidX.RecyclerView.Widget;

namespace Lct2023.Android.Decorations;

public class ColoredDividerItemDecoration : DividerItemDecoration
{
    private readonly int _orientation;

    public ColoredDividerItemDecoration(Context context, int orientation)
        : base(context, orientation)
    {
        _orientation = orientation;

        Padding = new Rect();
    }

    public Rect Padding { get; set; }

    public override void OnDraw(Canvas c, RecyclerView parent, RecyclerView.State state)
    {
        if (parent.GetLayoutManager() == null || Drawable == null)
        {
            return;
        }

        if (_orientation == Vertical)
        {
            DrawVertical(c, parent);
        }
        else
        {
            DrawHorizontal(c, parent);
        }
    }

    private void DrawVertical(Canvas canvas, RecyclerView parent)
    {
        canvas.Save();

        var left = Padding.Left;
        var right = parent.Width - Padding.Right;

        if (parent.ClipToPadding)
        {
            left += parent.PaddingLeft;
            right -= parent.PaddingRight;

            canvas.ClipRect(left, parent.PaddingTop, right, parent.Height - parent.PaddingBottom);
        }

        var bounds = new Rect();

        // Iterating through {count - 1} draws dividers only between items
        for (var i = 0; i < parent.ChildCount - 1; i++)
        {
            var child = parent.GetChildAt(i);
            parent.GetDecoratedBoundsWithMargins(child, bounds);
            var bottom = bounds.Bottom + Math.Round(child.TranslationY);
            var top = bottom - Drawable.IntrinsicHeight;

            Drawable.SetBounds(left, (int)top, right, (int)bottom);
            Drawable.Draw(canvas);
        }

        canvas.Restore();
    }

    private void DrawHorizontal(Canvas canvas, RecyclerView parent)
    {
        canvas.Save();

        var top = Padding.Top;
        var bottom = parent.Height - Padding.Bottom;

        if (parent.ClipToPadding)
        {
            top += parent.PaddingTop;
            bottom -= parent.PaddingBottom;
            canvas.ClipRect(parent.PaddingLeft, top, parent.Width - parent.PaddingRight, bottom);
        }

        var bounds = new Rect();

        // Iterating through {count - 1} draws dividers only between items
        for (var i = 0; i < parent.ChildCount - 1; i++)
        {
            var child = parent.GetChildAt(i);
            parent.GetLayoutManager().GetDecoratedBoundsWithMargins(child, bounds);
            var right = bounds.Right + Math.Round(child.TranslationX);
            var left = right - Drawable.IntrinsicWidth;

            Drawable.SetBounds((int)left, top, (int)right, bottom);
            Drawable.Draw(canvas);
        }

        canvas.Restore();
    }
}

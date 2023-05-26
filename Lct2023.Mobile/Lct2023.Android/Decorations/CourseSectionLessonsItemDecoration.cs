using System;
using Android.Content;
using Android.Graphics;
using AndroidX.RecyclerView.Widget;
using Lct2023.Android.Helpers;

namespace Lct2023.Android.Decorations;

public class CourseSectionLessonsItemDecoration : DividerItemDecoration
{
    public CourseSectionLessonsItemDecoration(Context context)
        : base(context, LinearLayoutManager.Vertical)
    {
        Drawable = context.GetDrawable(Resource.Drawable.course_section_lessons_item_decorator);
    }

    public override void OnDraw(Canvas canvas, RecyclerView parent, RecyclerView.State state)
    {
        if (parent.GetLayoutManager() == null || Drawable == null)
        {
            return;
        }

        canvas.Save();

        var left = DimensUtils.DpToPx(parent.Context, 48);
        var right = left + Drawable.IntrinsicWidth;

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
}
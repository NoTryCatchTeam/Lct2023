using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Lct2023.Android.Decorations;

public class GridItemDecoration : RecyclerView.ItemDecoration
{
    private readonly int _spanCount;
    private readonly int _spacing;

    public GridItemDecoration(int spanCount, int spacing)
    {
        _spanCount = spanCount;
        _spacing = spacing;
    }

    public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
    {
        var position = parent.GetChildAdapterPosition(view);
        var column = (view.LayoutParameters as StaggeredGridLayoutManager.LayoutParams)?.SpanIndex ??
                     (view.LayoutParameters as GridLayoutManager.LayoutParams)?.SpanIndex ??
                     position % _spanCount;

        outRect.Left = column * _spacing / _spanCount;
        outRect.Right = _spacing - (column + 1) * _spacing / _spanCount;

        if (position >= _spanCount)
        {
            outRect.Top = _spacing;
        }
    }
}

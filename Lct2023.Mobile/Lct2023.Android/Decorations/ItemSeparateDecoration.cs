using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Lct2023.Android.Decorations;

public class ItemSeparateDecoration : RecyclerView.ItemDecoration
{
    private readonly int _distanceBetweenItemsInPx;
    private readonly int _orientation;

    public ItemSeparateDecoration(int distanceBetweenItemsInPx, int orientation)
    {
        _distanceBetweenItemsInPx = distanceBetweenItemsInPx;
        _orientation = orientation;
    }

    public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
    {
        var itemNumber = parent.GetChildAdapterPosition(view);
        var distanceBetweenItems = itemNumber is 0 or RecyclerView.NoPosition ?
            0 :
            _distanceBetweenItemsInPx;

        if (_orientation == LinearLayoutManager.Horizontal)
        {
            outRect.Left = distanceBetweenItems;
        }
        else if (_orientation == LinearLayoutManager.Vertical)
        {
            outRect.Top = distanceBetweenItems;
        }
    }
}

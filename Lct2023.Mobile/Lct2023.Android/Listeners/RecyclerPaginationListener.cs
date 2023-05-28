using System.ComponentModel;
using System.Windows.Input;
using AndroidX.RecyclerView.Widget;

namespace Lct2023.Android.Listeners;

public class RecyclerPaginationListener: RecyclerView.OnScrollListener, INotifyPropertyChanged
{
    private readonly LinearLayoutManager _layoutManager;

    public RecyclerPaginationListener(LinearLayoutManager layoutManager)
    {
        _layoutManager = layoutManager;
    }

    public ICommand LoadMoreCommand { get; set; }

    public int LoadingOffset { get; set; }

    public bool IsLoadingMore { get; set; }

    public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
    {
        base.OnScrolled(recyclerView, dx, dy);

        if (_layoutManager.FindLastVisibleItemPosition() < (IsLoadingMore ? LoadingOffset - 1 : LoadingOffset))
        {
            return;
        }

        LoadMoreCommand?.Execute(null);
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
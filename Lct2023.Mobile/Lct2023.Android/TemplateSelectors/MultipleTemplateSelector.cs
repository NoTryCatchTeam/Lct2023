using System;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;

namespace Lct2023.Android.TemplateSelectors;

public class MultipleTemplateSelector<TItem> : IMvxTemplateSelector
{
    private readonly Func<TItem, int> _getItemViewType;
    private readonly Func<int, int> _getItemLayoutId;

    public MultipleTemplateSelector(Func<TItem, int> getItemViewType, Func<int, int> getItemLayoutId)
    {
        _getItemViewType = getItemViewType;
        _getItemLayoutId = getItemLayoutId;
    }

    int IMvxTemplateSelector.ItemTemplateId { get; set; }

    public int GetItemViewType(object forItemObject) =>
        forItemObject is not TItem item ? -1 : _getItemViewType(item);

    public int GetItemLayoutId(int fromViewType) =>
        _getItemLayoutId(fromViewType);
}

using Android.OS;
using Android.Views;
using Lct2023.ViewModels.Main;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class MainFragment : BaseFragment<MainViewModel>
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);
        
        var storiesLayout = view.FindViewById<MvxLinearLayout>(Resource.Id.stories_layout);
        storiesLayout.ItemTemplateId = Resource.Layout.StoryCard;
            
        var set = this.CreateBindingSet<MainFragment, MainViewModel>();
        
        set
            .Bind(storiesLayout)
            .For(v => v.ItemsSource)
            .To(vm => vm.StoryCards);

        set.Apply();

        return view;
    }

    
    protected override int GetLayoutId() => Resource.Layout.MainFragment;
}

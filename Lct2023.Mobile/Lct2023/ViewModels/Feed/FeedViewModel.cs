using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataModel.Definitions.Enums;
using DataModel.Responses.Art;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Feed;
using DataModel.Responses.Map;
using DynamicData;
using Lct2023.Business.RestServices.Art;
using Lct2023.Business.RestServices.Feed;
using Lct2023.Business.RestServices.Map;
using Lct2023.Commons.Extensions;
using Lct2023.Definitions.Enums;
using Lct2023.ViewModels.Common;
using Lct2023.ViewModels.Map;
using Lct2023.ViewModels.Map.Filters;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;
using ReactiveUI;

namespace Lct2023.ViewModels.Feed;

public class FeedViewModel : BaseViewModel
{
    private const int PAGE_SIZE = 20;
    
    private readonly IArtRestService _artRestService;
    private readonly IMapper _mapper;
    private readonly IFeedRestService _feedRestService;

    public FeedViewModel(
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService,
        IArtRestService artRestService,
        IFeedRestService feedRestService,
        IMapper mapper)
        : base(logFactory, navigationService)
    {
        _feedRestService = feedRestService;
        _artRestService = artRestService;
        _mapper = mapper;
        
        this.WhenAnyValue(vm => vm.SearchText, vm => vm.SelectedFilters)
            .Throttle(TimeSpan.FromMilliseconds(350), RxApp.MainThreadScheduler)
            .Subscribe(_ => UpdateItems());
    }

    public string Image { get; private set; } =
        "https://media.newyorker.com/photos/59095bb86552fa0be682d9d0/master/w_2560%2Cc_limit/Monkey-Selfie.jpg";

    public string SearchText { get; set; }
    
    public ObservableCollection<FeedItemViewModel> Items { get; } = new ();
    
    public ObservableCollection<FeedFilterGroupItemViewModel> FilterGroups { get; } = new ();
    
    public ObservableCollection<(FeedFilterGroupType FilterGroupType, string[] Items)> SelectedFilters { get; private set; }

    public void ApplyFilters()
    {
        SelectedFilters = FilterGroups
            .Select(group => (
                group.FilterGroupType,
                Items: group.Items?.Where(item => item.IsSelected).Select(item => item.Title).ToArray()))
            .Where(x => x.Items?.Any() == true)
            .ToObservableCollection();
    }
    
    public void ResetFilters()
    {
        FilterGroups.ForEach(group =>
            group?.Items?.ForEach(item => 
                item.IsSelected = false));

        SelectedFilters = null;
    }
    
    public override void ViewCreated()
    {
        base.ViewCreated();

        var feedTask = Task.Run(() => RunSafeTaskAsync(async () =>
        {
            var articles = (await _feedRestService.GetArticlesPaginationAsync(Items.Count, PAGE_SIZE, CancellationToken))?.Data?.ToArray();

            if (articles?.Any() != true)
            {
                return;
            }

            UpdateItems();
        }));
        
        var filtersTask = Task.Run(() => RunSafeTaskAsync(async () =>
        {
            IEnumerable<CmsItemResponse<RubricResponse>> rubrics = null;
            IEnumerable<CmsItemResponse<ArtCategoryResponse>> artCategories = null;
            
            
            await Task.WhenAll(Task.Run(async () => rubrics = (await _feedRestService.GetRubricsAsync(CancellationToken))?.ToArray()),
                Task.Run(async () => artCategories = (await _artRestService.GetArtCategoriesAsync(CancellationToken))?.ToArray()));

            artCategories
                ?.Where(ac => ac?.Item?.DisplayName != null)
                .Select(aC => aC.Item)
                .Then(aC => FilterGroups.Add(new FeedFilterGroupItemViewModel
                {
                    FilterGroupType = FeedFilterGroupType.ArtCategory,
                    Title = "Направления",
                    Items = _mapper.Map<ObservableCollection<FilterItemViewModel>>(aC),
                }));
                
            rubrics
                ?.Where(r => r?.Item?.Name != null)
                .Select(r => r.Item)
                .Then(r => FilterGroups.Add(new FeedFilterGroupItemViewModel
                {
                    FilterGroupType = FeedFilterGroupType.Rubrics,
                    Title = "Категории",
                    Items = _mapper.Map<ObservableCollection<FilterItemViewModel>>(r)
                }));
        }));

        Task.WhenAny(feedTask, filtersTask);
    }

    private void UpdateItems()
    {
        Items.Clear();
        
        if (SelectedFilters?.Any() == true)
        {
            return;
        }

    }
}

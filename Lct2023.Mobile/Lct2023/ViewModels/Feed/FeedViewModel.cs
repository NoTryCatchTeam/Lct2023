using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataModel.Responses.Art;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Feed;
using DynamicData;
using DynamicData.Binding;
using Lct2023.Business.RestServices.Art;
using Lct2023.Business.RestServices.Feed;
using Lct2023.Commons.Extensions;
using Lct2023.Definitions.Enums;
using Lct2023.Definitions.VmLinks;
using Lct2023.ViewModels.Common;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using ReactiveUI;
using Xamarin.Essentials;

namespace Lct2023.ViewModels.Feed;

public class FeedViewModel : BaseMainTabViewModel
{
    private const int PAGE_SIZE = 15;

    private readonly FeedItemViewModel _header = new();
    private readonly IArtRestService _artRestService;
    private readonly IMapper _mapper;
    private readonly IFeedRestService _feedRestService;
    private readonly IMvxCommand _itemClickCommand;
    private readonly IMvxCommand _itemExpandCommand;

    private CancellationTokenSource _feedCancellationTokenSource;


    private IEnumerable<CmsItemResponse<RubricResponse>> _rubrics;

    public IMvxCommand LoadMoreCommand { get; }
    
    public IMvxCommand UpdateItemsCommand { get; }

    public IMvxCommand ArtDirectionClickCommand { get; }

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
        
        _itemClickCommand = new MvxAsyncCommand<FeedItemViewModel>(vm =>
        {
            if (string.IsNullOrEmpty(vm.Link))
            {
                return Task.CompletedTask;
            }
                
            return RunSafeTaskAsync(() => Browser.OpenAsync(vm.Link, BrowserLaunchMode.SystemPreferred));
        });

        _itemExpandCommand = new MvxCommand<FeedItemViewModel>(vm => vm.Expanded = !vm.Expanded);

        ArtDirectionClickCommand = new MvxAsyncCommand<FeedArtDirectionItemViewModel>(vm => NavigationService.Navigate<ArtDirectionFeedViewModel, ArtDirectionFeedVmLink>(new ArtDirectionFeedVmLink
        {
            ArtDirection = vm.Title,
            Rubrics = _rubrics,
        }));
        
        UpdateItemsCommand = new MvxAsyncCommand(() => RunSafeTaskAsync(
            UpdateItemsAsync,
            _ =>
            {
                SetState();
                IsLoadMoreEnabled = false;
                IsLoadingMore = false;
                return Task.CompletedTask;
            }));
        
        LoadMoreCommand = new MvxAsyncCommand(() => RunSafeTaskAsync(() =>
        {
            IsLoadingMore = true;
            return LoadArticlesAsync();
        },
        _ =>
        {
            IsLoadMoreEnabled = false;
            IsLoadingMore = false;
            return Task.CompletedTask;
        }),() => IsLoadMoreEnabled && !IsLoadingMore && State is not (State.MinorLoading or State.Loading));
        
        this.WhenAnyValue(vm => vm.SearchText, vm => vm.SelectedFilters)
            .Skip(1)
            .Throttle(TimeSpan.FromMilliseconds(350), RxApp.MainThreadScheduler)
            .InvokeCommand(UpdateItemsCommand);
        
        Items
            .ObserveCollectionChanges()
            .Subscribe(_ => LoadingOffset = Items.Count - 4);
    }

    public State State { get; set; }
    
    public string SearchText { get; set; }
    
    public ObservableCollection<FeedItemViewModel> Items { get; } = new ();
    
    public ObservableCollection<FeedArtDirectionItemViewModel> ArtDirections { get; } = new ();
    
    public ObservableCollection<FeedFilterGroupItemViewModel> FilterGroups { get; } = new ();
    
    
    public bool IsLoadMoreEnabled { get; private set; }

    public bool IsLoadingMore { get; private set; }

    public int LoadingOffset { get; private set; }

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

        var filtersTask = Task.Run(() => RunSafeTaskAsync(async () =>
        {
            IEnumerable<CmsItemResponse<ArtCategoryResponse>> artCategories = null;


            await Task.WhenAll(
                Task.Run(async () => _rubrics = (await _feedRestService.GetRubricsAsync(CancellationToken))?.ToArray()),
                Task.Run(async () => artCategories = (await _artRestService.GetArtCategoriesAsync(CancellationToken))?.ToArray()));

            await InvokeOnMainThreadAsync(() =>
            {
                artCategories
                    ?.Where(ac => ac?.Item?.DisplayName != null)
                    .Select(aC => aC.Item)
                    .Then(aC =>
                    {
                        FilterGroups.Add(new FeedFilterGroupItemViewModel
                        {
                            FilterGroupType = FeedFilterGroupType.ArtCategory,
                            Title = "Направления",
                            Items = _mapper.Map<ObservableCollection<FilterItemViewModel>>(aC),
                        });

                        ArtDirections.AddRange(_mapper.Map<IEnumerable<FeedArtDirectionItemViewModel>>(aC));
                    });

                _rubrics
                    ?.Where(r => r?.Item?.Name != null)
                    .Select(r => r.Item)
                    .Then(r => FilterGroups.Add(new FeedFilterGroupItemViewModel
                    {
                        FilterGroupType = FeedFilterGroupType.Rubrics,
                        Title = "Категории",
                        Items = _mapper.Map<ObservableCollection<FilterItemViewModel>>(r)
                    }));
            });
        }));

        Task.WhenAny(RunSafeTaskAsync(UpdateItemsAsync,
            _ =>
            {
                SetState();
                IsLoadMoreEnabled = false;
                IsLoadingMore = false;
                return Task.CompletedTask;
            }),
            filtersTask);
    }

    public override void ViewDestroy(bool viewFinishing = true)
    {
        base.ViewDestroy(viewFinishing);

        _feedCancellationTokenSource?.Cancel();
        _feedCancellationTokenSource?.Dispose();
        _feedCancellationTokenSource = null;
    }

    private async Task UpdateItemsAsync()
    {
        State = State.MinorLoading;
        
        Items.Clear();

        Items.Add(_header);

        await LoadArticlesAsync();

        SetState();
    }
    
    private async Task LoadArticlesAsync()
    {
        _feedCancellationTokenSource?.Cancel();
        _feedCancellationTokenSource?.Dispose();
        _feedCancellationTokenSource = new CancellationTokenSource();
        
        var newArticles = (await _feedRestService.GetArticlesPaginationAsync(
            Items.Count - 1,
            PAGE_SIZE,
            SearchText,
            SelectedFilters?.Select(f => f.FilterGroupType switch
                {
                    FeedFilterGroupType.Rubrics => ("[rubric][name]", f.Items),
                    FeedFilterGroupType.ArtCategory => ("[art_categories][displayName]", f.Items),
                }).ToArray(),
            _feedCancellationTokenSource.Token))?.Data?.ToArray();

        if (newArticles?.Length > 0)
        {
            Items.AddRange(_mapper.Map<IEnumerable<FeedItemViewModel>>(newArticles, opts => opts.AfterMap((s, d) =>
                d.ForEach(vm => (vm.ExpandCommand, vm.ItemClickCommand) = (_itemExpandCommand, _itemClickCommand)))));
        }

        IsLoadMoreEnabled = newArticles?.Length >= PAGE_SIZE;
        IsLoadingMore = false;
    }

    private void SetState() => State = Items?.Count > 1 ? State.Default : State.NoData;
}

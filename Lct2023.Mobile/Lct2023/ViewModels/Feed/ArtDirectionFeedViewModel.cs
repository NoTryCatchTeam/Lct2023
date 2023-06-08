using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DataModel.Definitions.Enums;
using DataModel.Responses.Art;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Feed;
using DynamicData;
using DynamicData.Binding;
using Lct2023.Business.RestServices.Art;
using Lct2023.Business.RestServices.Feed;
using Lct2023.Business.RestServices.Map;
using Lct2023.Commons.Extensions;
using Lct2023.Definitions.Enums;
using Lct2023.Definitions.VmLinks;
using Lct2023.ViewModels.Common;
using Lct2023.ViewModels.Map;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using ReactiveUI;
using Xamarin.Essentials;
using Lct2023.Business.Definitions;
using System.Numerics;

namespace Lct2023.ViewModels.Feed
{
	public class ArtDirectionFeedViewModel : BaseViewModel<ArtDirectionFeedVmLink>
    {
        private const int PAGE_SIZE = 15;

        private readonly FeedItemViewModel _header = new();
        private readonly IArtRestService _artRestService;
        private readonly IMapper _mapper;
        private readonly IMapRestService _mapRestService;
        private readonly IFeedRestService _feedRestService;
        private readonly IMvxCommand _itemClickCommand;
        private readonly IMvxCommand _itemExpandCommand;

        private Task<IEnumerable<PlaceItemViewModel>> loadPlacesTask;

        private CancellationTokenSource _feedCancellationTokenSource;

        public IMvxCommand LoadMoreCommand { get; }

        public IMvxCommand UpdateItemsCommand { get; }

        public IMvxCommand OpenMapCommand { get; }

        public ArtDirectionFeedViewModel(
            ILoggerFactory logFactory,
            IMvxNavigationService navigationService,
            IArtRestService artRestService,
            IFeedRestService feedRestService,
            IMapper mapper,
            IMapRestService mapRestService)
            : base(logFactory, navigationService)
        {
            _mapRestService = mapRestService;
            _feedRestService = feedRestService;
            _artRestService = artRestService;
            _mapper = mapper;

            OpenMapCommand = new MvxAsyncCommand(vm => NavigationService.Navigate<LocationsMapViewModel, LocationsMapVmLink>(
                new LocationsMapVmLink
                {
                    LocationType = DataModel.Definitions.Enums.LocationType.School,
                    PlacesFactory = () => loadPlacesTask ?? Task.FromResult(default(IEnumerable<PlaceItemViewModel>)),
                }));

            _itemClickCommand = new MvxAsyncCommand<FeedItemViewModel>(vm =>
            {
                if (string.IsNullOrEmpty(vm.Link))
                {
                    return Task.CompletedTask;
                }

                return RunSafeTaskAsync(() => Browser.OpenAsync(vm.Link, BrowserLaunchMode.SystemPreferred));
            });

            _itemExpandCommand = new MvxCommand<FeedItemViewModel>(vm => vm.Expanded = !vm.Expanded);

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
            }), () => IsLoadMoreEnabled && !IsLoadingMore && State is not (State.MinorLoading or State.Loading));

            this.WhenAnyValue(vm => vm.SearchText, vm => vm.SelectedFilters)
                .Skip(1)
                .Throttle(TimeSpan.FromMilliseconds(350), RxApp.MainThreadScheduler)
                .InvokeCommand(UpdateItemsCommand);

            Items
                .ObserveCollectionChanges()
                .Subscribe(_ => LoadingOffset = Items.Count - 5);
        }

        public State State { get; set; }

        public string SearchText { get; set; }

        private string[] _rubrics;

        public string SelectedArtDirection { get; set; }

        public string ArtDirectionFeedDescription => string.Format("Новости из сферы \"{0}\": {1}", SelectedArtDirection, _rubrics?.Then(r => string.Join(", ", r)).ToLower());

        public ObservableCollection<FeedItemViewModel> Items { get; } = new();

        public ObservableCollection<FeedFilterGroupItemViewModel> FilterGroups { get; } = new();


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

        public override Task Initialize()
        {
            loadPlacesTask = Task.Run(async () => (await _mapRestService.LoadUntilEndAsync((rS, start) => rS.GetSchoolsLocationPaginationAsync(start, PAGE_SIZE, CancellationToken, new[] { SelectedArtDirection })))?.Then(sLs => _mapper.Map<IEnumerable<PlaceItemViewModel>>(sLs)));

            return Task.WhenAny(RunSafeTaskAsync(UpdateItemsAsync,
                _ =>
                {
                    SetState();
                    IsLoadMoreEnabled = false;
                    IsLoadingMore = false;
                    return Task.CompletedTask;
                }),
                base.Initialize());
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);

            _feedCancellationTokenSource?.Cancel();
            _feedCancellationTokenSource?.Dispose();
            _feedCancellationTokenSource = null;
        }

        public override void Prepare(ArtDirectionFeedVmLink parameter)
        {
            base.Prepare(parameter);
            _rubrics = parameter
                .Rubrics
                ?.Where(r => r?.Item?.Name != null)
                .Select(r => r.Item.Name)
                .ToArray();

            SelectedArtDirection = parameter.ArtDirection;
            parameter
                .Rubrics
                ?.Where(r => r?.Item?.Name != null)
                .Select(r => r.Item)
                .Then(r => FilterGroups.Add(new FeedFilterGroupItemViewModel
                {
                    FilterGroupType = FeedFilterGroupType.Rubrics,
                    Title = "Категории",
                    Items = _mapper.Map<ObservableCollection<FilterItemViewModel>>(r)
                }));
        }

        private async Task UpdateItemsAsync()
        {
            State = State.MinorLoading;

            Items.Clear();

            Items.Add(_header);
            Items.Add(_header);

            await LoadArticlesAsync();

            SetState();
        }

        private async Task LoadArticlesAsync()
        {
            _feedCancellationTokenSource?.Cancel();
            _feedCancellationTokenSource?.Dispose();
            _feedCancellationTokenSource = new CancellationTokenSource();

            List<(string field, string[] values)> filters = new()
            {
                ("[art_categories][displayName]", new []{ SelectedArtDirection })
            };
            SelectedFilters?.Select(f => f.FilterGroupType switch
            {
                FeedFilterGroupType.Rubrics => ("[rubric][name]", f.Items),
                FeedFilterGroupType.ArtCategory => ("[art_categories][displayName]", f.Items),
            })?.Then(selectedFilters => filters.AddRange(selectedFilters));

            var newArticles = (await _feedRestService.GetArticlesPaginationAsync(
                Items.Count - 2,
                PAGE_SIZE,
                SearchText,
                filters,
                _feedCancellationTokenSource.Token))?.Data?.ToArray();

            if (newArticles?.Length > 0)
            {
                Items.AddRange(_mapper.Map<IEnumerable<FeedItemViewModel>>(newArticles, opts => opts.AfterMap((s, d) =>
                    d.ForEach(vm => (vm.ExpandCommand, vm.ItemClickCommand) = (_itemExpandCommand, _itemClickCommand)))));
            }

            IsLoadMoreEnabled = newArticles?.Length >= PAGE_SIZE;
            IsLoadingMore = false;
        }

        private void SetState() => State = Items?.Count > 2 ? State.Default : State.NoData;
    }
}


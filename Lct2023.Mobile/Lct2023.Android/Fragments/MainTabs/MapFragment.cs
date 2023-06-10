using System.Reactive.Disposables;
using Android.Gms.Maps;
using Android.OS;
using Android.Views;
using Lct2023.ViewModels.Map;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using DataModel.Definitions.Enums;
using DynamicData.Binding;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Google.Android.Material.TextField;
using Lct2023.Android.Adapters;
using Lct2023.Android.Bindings;
using Lct2023.Android.Decorations;
using Lct2023.Android.Definitions.Extensions;
using Lct2023.Android.Helpers;
using Lct2023.Android.TemplateSelectors;
using Lct2023.Converters;
using Lct2023.Definitions.Enums;
using MvvmCross.Binding.ValueConverters;
using MvvmCross.Commands;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using ReactiveUI;
using Lct2023.Android.Listeners;
using Lct2023.Android.Views;
using Lct2023.Android.Callbacks;
using AndroidX.CoordinatorLayout.Widget;
using MvvmCross.Platforms.Android.Presenters;
using Lct2023.Android.Definitions.Models.Map;
using Android.Gms.Maps.Utils.Clustering;
using static Android.Gms.Maps.Utils.Clustering.ClusterManager;
using MvvmCross.Binding.BindingContext;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class MapFragment : BaseMainTabFragment<MapViewModel>, IOnMapReadyCallback, View.IOnClickListener, GoogleMap.IOnMapClickListener, ClusterManager.IOnClusterItemClickListener,
    IOnClusterClickListener
{
    private const float MAX_DIM_ALPHA = 0.5f;
    private const int LAT_LNG_BOUNDS_PADDING = 100;

    private MapView _mapView;

    private GoogleMap _googleMap;
    private ClusterManager _clusterManager;
    private BottomSheetBehavior _locationDetailsBottomSheetBehavior;
    private MaterialCardView _locationDetailsBottomSheet;
    private View _addressLayout;
    private View _titleLayout;
    private MaterialCardView _filtersBottomSheet;
    private BottomSheetBehavior _filtersBottomSheetBehavior;
    private CoordinatorLayout _mapContainer;

    protected override int GetLayoutId() => Resource.Layout.MapFragment;

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);

        Toolbar.Title = "Карта";

        _mapView = view.FindViewById<MapView>(Resource.Id.map_view);
        _mapContainer = view.FindViewById<CoordinatorLayout>(Resource.Id.map_container);
        var locationsSearchEditText = view.FindViewById<TextInputEditText>(Resource.Id.locations_search_value);
        var searchResultsList = view.FindViewById<MvxRecyclerView>(Resource.Id.map_search_results);
        var locationTypesGroup = view.FindViewById<SegmentedControl>(Resource.Id.location_types_group);
        var filtersButton = view.FindViewById<MaterialButton>(Resource.Id.location_filters_button);
        var zoomInButton = view.FindViewById<MaterialButton>(Resource.Id.zoom_in_button);
        var zoomOutButton = view.FindViewById<MaterialButton>(Resource.Id.zoom_out_button);
        _locationDetailsBottomSheet = view.FindViewById<MaterialCardView>(Resource.Id.location_info_bottom_sheet);
        _locationDetailsBottomSheetBehavior = BottomSheetBehavior.From(_locationDetailsBottomSheet);

        var closeBsButton = view.FindViewById<MaterialButton>(Resource.Id.close_bs_button);
        var contactsItemsLayout = view.FindViewById<MvxLinearLayout>(Resource.Id.contacts_items_layout);
        var moreContactsButton = view.FindViewById<MaterialButton>(Resource.Id.more_contacts_button);

        _addressLayout = view.FindViewById(Resource.Id.location_address_layout);
        _titleLayout = view.FindViewById(Resource.Id.location_title_layout);
        var addressTextView = view.FindViewById<TextView>(Resource.Id.address_text_view);
        var mailTextView = view.FindViewById<TextView>(Resource.Id.mail_text_view);
        var siteTextView = view.FindViewById<TextView>(Resource.Id.site_text_view);

        var socialLinksLayout = view.FindViewById<MvxRecyclerView>(Resource.Id.open_social_nets_items_layout);
        var artDirectionsLayout = view.FindViewById<MvxRecyclerView>(Resource.Id.art_directions_layout);
        var afishaLayout = view.FindViewById<MvxRecyclerView>(Resource.Id.afisha_layout);
        var actionButton = view.FindViewById<MaterialButton>(Resource.Id.action_button);
        _filtersBottomSheet = view.FindViewById<MaterialCardView>(Resource.Id.filters_bottom_sheet);
        _filtersBottomSheetBehavior = BottomSheetBehavior.From(_filtersBottomSheet);
        _filtersBottomSheetBehavior.FitToContents = true;
        var dimView = view.FindViewById(Resource.Id.map_dim);
        var mapFiltersRecycle = view.FindViewById<MvxRecyclerView>(Resource.Id.map_filters_recycle);
        var filtersCloseBsButton = view.FindViewById<MaterialButton>(Resource.Id.map_filters_close_bs_button);
        var clearFiltersButton = view.FindViewById<MaterialButton>(Resource.Id.map_clear_filters_button);
        var applyFiltersButton = view.FindViewById<MaterialButton>(Resource.Id.map_apply_filters_button);

        var artDirectionsAdapter = new StreamsAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.ArtDirectionItem),
        };

        var horizontal16dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 16), LinearLayoutManager.Horizontal);
        var horizontal8dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 8), LinearLayoutManager.Horizontal);
        var vertical16dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 16), LinearLayoutManager.Vertical);

        artDirectionsLayout.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Horizontal });
        artDirectionsLayout.ItemTemplateId = Resource.Layout.ArtDirectionItem;
        artDirectionsLayout.SetAdapter(artDirectionsAdapter);
        artDirectionsLayout.AddItemDecoration(horizontal8dpItemSpacingDecoration);

        contactsItemsLayout.ItemTemplateId = Resource.Layout.ContactItem;

        var afishaAdapter = new EventsAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.AfishItem),
        };

        afishaLayout.ItemTemplateId = Resource.Layout.AfishItem;
        afishaLayout.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Horizontal });
        afishaLayout.AddItemDecoration(horizontal8dpItemSpacingDecoration);
        afishaLayout.SetAdapter(afishaAdapter);

        var socialLinksAdapter = new SocialLinksAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.SocialLinktem),
        };

        socialLinksLayout.ItemTemplateId = Resource.Layout.SocialLinktem;
        socialLinksLayout.SetLayoutManager(new MvxGuardedLinearLayoutManager(Activity) { Orientation = LinearLayoutManager.Horizontal });
        socialLinksLayout.AddItemDecoration(horizontal16dpItemSpacingDecoration);
        socialLinksLayout.SetAdapter(socialLinksAdapter);

        var searchAdapter = new MapSearchResultsAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MultipleTemplateSelector<MapSearchResultItemViewModel>(
                item => item.LocationType switch
                {
                    LocationType.Event => Resource.Layout.EventSearchResultItemView,
                    LocationType.School => Resource.Layout.SchoolSearchResultItemView,
                },
                id => id),
        };

        searchResultsList.SetAdapter(searchAdapter);

        var mapFiltersAdapter = new MapFiltersGroupsListAdapter((IMvxAndroidBindingContext)BindingContext, () => _mapContainer.Post(() => _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateExpanded))
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.MapFiltersGroupItemView),
        };

        mapFiltersRecycle.SetLayoutManager(new MvxGuardedLinearLayoutManager(Context) { Orientation = LinearLayoutManager.Vertical });
        mapFiltersRecycle.SetAdapter(mapFiltersAdapter);
        mapFiltersRecycle.AddItemDecoration(vertical16dpItemSpacingDecoration);

        dimView.SetOnTouchListener(new DefaultOnTouchListener((v, e) =>
        {
            var isExpanded = _filtersBottomSheetBehavior.State == BottomSheetBehavior.StateExpanded || _locationDetailsBottomSheetBehavior.State == BottomSheetBehavior.StateExpanded;

            if (isExpanded || _locationDetailsBottomSheetBehavior.State == BottomSheetBehavior.StateCollapsed)
            {
                DeselectLocation();
            }

            return isExpanded;
        }));

        var bottomSheetCallback = new DefaultBottomSheetCallback(
            (v, s) => dimView.Alpha = Math.Min(s, MAX_DIM_ALPHA));

        _filtersBottomSheetBehavior.AddBottomSheetCallback(bottomSheetCallback);

        _locationDetailsBottomSheetBehavior.AddBottomSheetCallback(bottomSheetCallback);

        searchAdapter.ItemClick = new MvxCommand<MapSearchResultItemViewModel>(
            item =>
            {
                locationsSearchEditText.Text = null;
                KeyboardUtils.HideKeyboard(Activity);
                SelectLocation(item.Id);

                if (ViewModel.SelectedLocation == null)
                {
                    return;
                }

                try
                {
                    _googleMap?.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(ViewModel.SelectedLocation.Latitude, ViewModel.SelectedLocation.Longitude), 16));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

        _locationDetailsBottomSheetBehavior.Hideable = true;
        _locationDetailsBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;

        foreach (var button in new[] { filtersButton, closeBsButton, zoomInButton, zoomOutButton, filtersCloseBsButton, applyFiltersButton, clearFiltersButton })
        {
            button.SetOnClickListener(this);
        }

        _mapView?.OnCreate(savedInstanceState);
        _mapView?.GetMapAsync(this);

        ViewModel.Places
            .ObserveCollectionChanges()
            .Throttle(TimeSpan.FromMilliseconds(150), RxApp.MainThreadScheduler)
            .Subscribe(_ => UpdateMarkers())
            .DisposeWith(CompositeDisposable);

        var set = CreateBindingSet();

        set
            .Bind(locationsSearchEditText)
            .For(v => v.Text)
            .To(vm => vm.SearchText);

        set
            .Bind(view.FindViewById<TextView>(Resource.Id.location_title))
            .For(v => v.Text)
            .To(vm => vm.SelectedLocation.Title);

        set
            .Bind(view.FindViewById<TextView>(Resource.Id.location_description))
            .For(v => v.Text)
            .To(vm => vm.SelectedLocation.Description);

        set
            .Bind(addressTextView)
            .For(v => v.Text)
            .To(vm => vm.SelectedLocation.Address);

        set
            .Bind(mailTextView)
            .For(v => v.BindLongClick())
            .To(vm => vm.CopyCommand)
            .WithConversion<MvxCommandParameterValueConverter>(MapViewModel.LocationActionItem.Email);

        set
            .Bind(mailTextView)
            .For(v => v.Text)
            .To(vm => vm.SelectedLocation.Email);

        set
            .Bind(mailTextView)
            .For(v => v.BindClick())
            .To(vm => vm.OpenEmailCommand)
            .WithConversion<MvxCommandParameterValueConverter>(MapViewModel.LocationActionItem.Email);

        set
            .Bind(addressTextView)
            .For(v => v.BindLongClick())
            .To(vm => vm.CopyCommand)
            .WithConversion<MvxCommandParameterValueConverter>(MapViewModel.LocationActionItem.Address);

        set
            .Bind(addressTextView)
            .For(v => v.BindClick())
            .To(vm => vm.OpenMapCommand);

        set
            .Bind(siteTextView)
            .For(v => v.BindLongClick())
            .To(vm => vm.CopyCommand)
            .WithConversion<MvxCommandParameterValueConverter>(MapViewModel.LocationActionItem.Site);

        set
            .Bind(siteTextView)
            .For(v => v.Text)
            .To(vm => vm.SelectedLocation.Site);

        set
            .Bind(siteTextView)
            .For(v => v.BindClick())
            .To(vm => vm.OpenSiteCommand)
            .WithConversion<MvxCommandParameterValueConverter>(MapViewModel.LocationActionItem.Site);

        set
            .Bind(view.FindViewById<MaterialButton>(Resource.Id.gps_button))
            .For(v => v.BindClick())
            .To(vm => vm.GpsCommand);

        var visibilityConverter = new AnyExpressionConverter<string, bool>(text => !string.IsNullOrEmpty(text));

        set
            .Bind(view.FindViewById(Resource.Id.site_layout))
            .For(v => v.BindVisible())
            .To(vm => vm.SelectedLocation.Site)
            .WithConversion(new AnyExpressionConverter<string, bool>(
                text => ViewModel.SelectedLocation?.LocationType == LocationType.Event && !string.IsNullOrEmpty(text)));

        set
            .Bind(view.FindViewById(Resource.Id.site_bottom_line))
            .For(v => v.BindVisible())
            .To(vm => vm.SelectedLocation.Site)
            .WithConversion(new AnyExpressionConverter<string, bool>(
                text => ViewModel.SelectedLocation?.LocationType == LocationType.Event && !string.IsNullOrEmpty(text)));

        var itemsVisibilityConverter = new AnyExpressionConverter<IEnumerable<object>, bool>(items => items?.Any() == true);

        set
            .Bind(view.FindViewById(Resource.Id.contacts_layout))
            .For(v => v.BindVisible())
            .To(vm => vm.SelectedLocation.Contacts)
            .WithConversion(itemsVisibilityConverter);

        set
            .Bind(view.FindViewById(Resource.Id.contacts_bottom_line))
            .For(v => v.BindVisible())
            .To(vm => vm.SelectedLocation.Contacts)
            .WithConversion(itemsVisibilityConverter);

        set
            .Bind(view.FindViewById(Resource.Id.mail_layout))
            .For(v => v.BindVisible())
            .To(vm => vm.SelectedLocation.Email)
            .WithConversion(visibilityConverter);

        set
            .Bind(view.FindViewById(Resource.Id.mail_bottom_line))
            .For(v => v.BindVisible())
            .To(vm => vm.SelectedLocation.Email)
            .WithConversion(visibilityConverter);

        set
            .Bind(socialLinksLayout)
            .For(v => v.ItemsSource)
            .To(vm => vm.SocialLinks);

        set
            .Bind(artDirectionsLayout)
            .For(v => v.ItemsSource)
            .To(vm => vm.SelectedLocation.Streams);

        set
            .Bind(contactsItemsLayout)
            .For(v => v.ItemsSource)
            .To(vm => vm.Contacts);

        set
            .Bind(searchResultsList)
            .For(v => v.ItemsSource)
            .To(vm => vm.SearchResults);

        set
            .Bind(searchResultsList)
            .For(v => v.BindVisible())
            .To(vm => vm.SearchText)
            .WithConversion(visibilityConverter);

        set
            .Bind(artDirectionsLayout)
            .For(v => v.BindVisible())
            .To(vm => vm.SelectedLocation.Streams)
            .WithConversion(itemsVisibilityConverter);

        set
            .Bind(view.FindViewById<MaterialButton>(Resource.Id.open_address_button))
            .For(v => v.BindClick())
            .To(vm => vm.OpenMapCommand);

        set
            .Bind(view.FindViewById(Resource.Id.events_header_text_view))
            .For(v => v.BindVisible())
            .To(vm => vm.Events)
            .WithConversion(itemsVisibilityConverter);

        set
            .Bind(afishaLayout)
            .For(v => v.BindVisible())
            .To(vm => vm.Events)
            .WithConversion(itemsVisibilityConverter);

        set
            .Bind(afishaLayout)
            .For(v => v.ItemsSource)
            .To(vm => vm.Events);

        set
            .Bind(moreContactsButton)
            .For(v => v.BindVisible())
            .To(vm => vm.SelectedLocation.Contacts)
            .WithConversion(new AnyExpressionConverter<IEnumerable<string>, bool>(items => items?.Count() > 1));

        set
            .Bind(filtersButton)
            .For(nameof(ButtonIconResourceBinding))
            .To(vm => vm.SelectedFilters)
            .WithConversion(new AnyExpressionConverter<ObservableCollection<(MapFilterGroupType FilterGroupType, string[] Items)>, int>(filters =>
                filters?.Any() == true ? Resource.Drawable.ic_filters_selected : Resource.Drawable.ic_filters));

        set
            .Bind(locationTypesGroup)
            .For(v => v.SelectedSegment)
            .To(vm => vm.LocationType)
            .WithConversion(new AnyExpressionConverter<LocationType, int>(locationType => (int)locationType, selectedSegment => (LocationType)selectedSegment));

        set
            .Bind(moreContactsButton)
            .For(v => v.BindClick())
            .To(vm => vm.ExpandContactsCommand);

        set
            .Bind(moreContactsButton)
            .For(v => v.Text)
            .To(vm => vm.Contacts)
            .WithConversion(new AnyExpressionConverter<IEnumerable<ContactItemViewModel>, string>(items => items?.Count() > 1 ? "Свернуть" : "Eщё"));

        set
            .Bind(actionButton)
            .For(v => v.Text)
            .To(vm => vm.SelectedLocation.LocationType)
            .WithConversion(new AnyExpressionConverter<LocationType, string>(type => type switch
            {
                LocationType.Event => "Расписание и билеты",
                LocationType.School => "Записаться на урок",
            }));

        set
            .Bind(actionButton)
            .For(v => v.BindClick())
            .To(vm => vm.ActionCommand);

        set
            .Bind(mapFiltersRecycle)
            .For(v => v.ItemsSource)
            .To(vm => vm.FilterGroups);

        set.Apply();

        this.WhenAnyValue(v => v.ViewModel.MyPosition)
            .WhereNotNull()
            .Subscribe(_ =>
            {
                try
                {
                    _googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(ViewModel.MyPosition.Latitude, ViewModel.MyPosition.Longitude), 14));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            })
            .DisposeWith(CompositeDisposable);

        return view;
    }

    public override void OnStart()
    {
        base.OnStart();
        _mapView?.OnStart();
    }

    public override void OnResume()
    {
        base.OnResume();
        _mapView?.OnResume();
    }

    public override void OnPause()
    {
        _mapView?.OnPause();
        base.OnPause();
    }

    public override void OnStop()
    {
        _mapView?.OnStop();
        base.OnStop();
    }

    public override void OnDestroy()
    {
        _mapView?.OnDestroy();
        base.OnDestroy();
    }

    public override void OnLowMemory()
    {
        base.OnLowMemory();
        _mapView?.OnLowMemory();
    }

    public void OnMapReady(GoogleMap googleMap)
    {
        _googleMap = googleMap;

        _clusterManager = new ClusterManager(Context, googleMap);
        _clusterManager.Renderer = new MapClusterRenderer(Context, googleMap, _clusterManager)
        {
            MinClusterSize = 2,
        };

        var set = CreateBindingSet();

        set.Bind(_googleMap)
            .For(x => x.MyLocationEnabled)
            .To(vm => vm.IsMyLocationEnabled);

        set.Apply();

        _googleMap.SetOnCameraIdleListener(_clusterManager);
        _googleMap.SetOnMarkerClickListener(_clusterManager);
        _googleMap.SetOnMapClickListener(this);
        _clusterManager.SetOnClusterItemClickListener(this);
        _clusterManager.SetOnClusterClickListener(this);

        _googleMap.MapType = GoogleMap.MapTypeNormal;
        _googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(55.7499931, 37.624216), 9));

        _googleMap.UiSettings.CompassEnabled = false;
        _googleMap.UiSettings.ZoomControlsEnabled = false;
        _googleMap.UiSettings.MyLocationButtonEnabled = false;

        UpdateMarkers();
    }

    private void UpdateMarkers()
    {
        KeyboardUtils.HideKeyboard(Activity);
        DeselectLocation();

        if (_googleMap == null)
        {
            return;
        }

        _clusterManager.ClearItems();

        foreach (var place in ViewModel.Places)
        {
            _clusterManager.AddItem(new MapClusterItem(place));
        }

        _clusterManager.Cluster();
    }

    public void OnClick(View view)
    {
        switch (view.Id)
        {
            case Resource.Id.map_apply_filters_button:
                _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                ViewModel.ApplyFilters();

                break;
            case Resource.Id.map_clear_filters_button:
                _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
                ViewModel.ResetFilters();

                break;
            case Resource.Id.location_filters_button:
                _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;

                break;
            case Resource.Id.map_filters_close_bs_button:
                _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;

                break;
            case Resource.Id.close_bs_button:
                DeselectLocation();

                break;
            case Resource.Id.zoom_in_button:
                try
                {
                    _googleMap.AnimateCamera(CameraUpdateFactory.ZoomIn());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                break;
            case Resource.Id.zoom_out_button:
                try
                {
                    _googleMap.AnimateCamera(CameraUpdateFactory.ZoomOut());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                break;
        }
    }

    private void DeselectLocation()
    {
        _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;

        if (ViewModel.SelectedLocation == null)
        {
            return;
        }

        ViewModel.SelectedLocation = null;
        _locationDetailsBottomSheetBehavior.PeekHeight = 0;
        _locationDetailsBottomSheetBehavior.Hideable = true;
        _locationDetailsBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
    }

    private void SelectLocation(string id)
    {
        ViewModel.SelectedLocation = ViewModel.Places?.FirstOrDefault(place => place.Id == id);

        if (ViewModel.SelectedLocation == null)
        {
            return;
        }

        _mapContainer.Post(() =>
        {
            _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
            var addressRect = new Rect();
            GetOffsetDescendant(ViewModel.LocationType == LocationType.School ? _addressLayout : _titleLayout);
            _locationDetailsBottomSheetBehavior.SetPeekHeight(addressRect.Top + addressRect.Height() + DimensUtils.DpToPx(Context, 8), false);
            _locationDetailsBottomSheetBehavior.Hideable = false;
            _locationDetailsBottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;

            void GetOffsetDescendant(View view)
            {
                view.GetDrawingRect(addressRect);
                _locationDetailsBottomSheet.OffsetDescendantRectToMyCoords(view, addressRect);
            }
        });
    }

    public void OnMapClick(LatLng point) => DeselectLocation();

    public bool OnClusterItemClick(Java.Lang.Object item)
    {
        if (item is MapClusterItem clusterItem)
        {
            SelectLocation(clusterItem.Snippet);
        }

        return true;
    }

    public bool OnClusterClick(ICluster cluster)
    {
        var builder = new LatLngBounds.Builder();

        foreach (IClusterItem item in cluster.Items)
        {
            builder.Include(item.Position);
        }

        _googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(builder.Build(), LAT_LNG_BOUNDS_PADDING));

        return true;
    }
}

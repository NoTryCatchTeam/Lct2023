using System.Reactive.Disposables;
using Android.Gms.Maps;
using Android.OS;
using Android.Views;
using Lct2023.ViewModels.Map;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Widget;
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
using MvvmCross.Binding.ValueConverters;
using MvvmCross.Commands;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using ReactiveUI;
using Square.Picasso;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class MapFragment : BaseFragment<MapViewModel>, IOnMapReadyCallback, View.IOnClickListener, GoogleMap.IOnMarkerClickListener, GoogleMap.IOnMapClickListener
{
    private MapView _mapView;

    private GoogleMap _googleMap;
    private BottomSheetBehavior _locationDetailsBottomSheetBehavior;
    private MaterialCardView _locationDetailsBottomSheet;
    private View _addressLayout;
    private MaterialCardView _filtersBottomSheet;
    private BottomSheetBehavior _filtersBottomSheetBehavior;

    protected override int GetLayoutId() => Resource.Layout.MapFragment;
    
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);
        
        view.FindViewById<TextView>(Resource.Id.title).Text = "Карта";
        _mapView = view.FindViewById<MapView>(Resource.Id.map_view);
        
        var locationsSearchEditText = view.FindViewById<TextInputEditText>(Resource.Id.locations_search_value);
        var searchResultsList = view.FindViewById<MvxRecyclerView>(Resource.Id.map_search_results);
        var locationTypesGroup = view.FindViewById<MaterialButtonToggleGroup>(Resource.Id.location_types_group);
        var avatarImageButton = view.FindViewById<ImageView>(Resource.Id.toolbar_image);
        var filtersButton = view.FindViewById<MaterialButton>(Resource.Id.location_filters_button);
        var zoomInButton = view.FindViewById<MaterialButton>(Resource.Id.zoom_in_button);
        var zoomOutButton = view.FindViewById<MaterialButton>(Resource.Id.zoom_out_button);
        _locationDetailsBottomSheet = view.FindViewById<MaterialCardView>(Resource.Id.location_info_bottom_sheet);
        _locationDetailsBottomSheetBehavior = BottomSheetBehavior.From(_locationDetailsBottomSheet);
        
        var closeBsButton = view.FindViewById<MaterialButton>(Resource.Id.close_bs_button);
        var contactsItemsLayout = view.FindViewById<MvxLinearLayout>(Resource.Id.contacts_items_layout);
        var moreContactsButton = view.FindViewById<MaterialButton>(Resource.Id.more_contacts_button);
        
        _addressLayout = view.FindViewById(Resource.Id.location_address_layout);
        var addressTextView = view.FindViewById<TextView>(Resource.Id.address_text_view);
        var mailTextView = view.FindViewById<TextView>(Resource.Id.mail_text_view);
        var siteTextView = view.FindViewById<TextView>(Resource.Id.site_text_view);
        
        var socialLinksLayout = view.FindViewById<MvxRecyclerView>(Resource.Id.open_social_nets_items_layout);
        var artDirectionsLayout = view.FindViewById<MvxRecyclerView>(Resource.Id.art_directions_layout);
        var afishaLayout = view.FindViewById<MvxRecyclerView>(Resource.Id.afisha_layout);
        var actionButton = view.FindViewById<MaterialButton>(Resource.Id.action_button);
        _filtersBottomSheet = view.FindViewById<MaterialCardView>(Resource.Id.filters_bottom_sheet);
        _filtersBottomSheetBehavior = BottomSheetBehavior.From(_filtersBottomSheet);
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
        
        var mapFiltersAdapter = new MapFiltersGroupsListAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.MapFiltersGroupItemView),
        };

        mapFiltersRecycle.SetLayoutManager(new MvxGuardedLinearLayoutManager(Context) { Orientation = LinearLayoutManager.Vertical });
        mapFiltersRecycle.SetAdapter(mapFiltersAdapter);
        mapFiltersRecycle.AddItemDecoration(vertical16dpItemSpacingDecoration);
        
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

        foreach (var button in new [] { filtersButton, closeBsButton, zoomInButton, zoomOutButton, filtersCloseBsButton, applyFiltersButton, clearFiltersButton })
        {
            button.SetOnClickListener(this);
        }
        
        Picasso.Get()
            .Load(ViewModel.Image)
            .Into(avatarImageButton);
        
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
            .WithConversion(visibilityConverter);
        
        set
            .Bind(view.FindViewById(Resource.Id.site_bottom_line))
            .For(v => v.BindVisible())
            .To(vm => vm.SelectedLocation.Site)
            .WithConversion(visibilityConverter);
        
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
            .WithConversion(new AnyExpressionConverter<object, int>(filters => filters == null ? Resource.Drawable.ic_filters : Resource.Drawable.ic_filters_selected));

        set
            .Bind(locationTypesGroup)
            .For(v => v.CheckedButtonId)
            .To(vm => vm.LocationType)
            .WithConversion(new AnyExpressionConverter<LocationType, int>(
                locationType => locationType switch
                {
                    LocationType.School => Resource.Id.msa_option,
                    LocationType.Event => Resource.Id.afisha_option,
                }));
        
        Observable.FromEventPattern<EventHandler<MaterialButtonToggleGroup.ButtonCheckedEventArgs>, MaterialButtonToggleGroup.ButtonCheckedEventArgs>(
                h => locationTypesGroup.ButtonChecked += h,
                h => locationTypesGroup.ButtonChecked -= h)
            .Where(_ => locationTypesGroup.CheckedButtonId != -1)
            .Select(_ => locationTypesGroup.CheckedButtonId)
            .Subscribe(buttonId => ViewModel.LocationType = buttonId switch
            {
                Resource.Id.msa_option => LocationType.School,
                Resource.Id.afisha_option => LocationType.Event,
            })
            .DisposeWith(CompositeDisposable);

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
        
        var set = CreateBindingSet();

        set.Bind(_googleMap)
            .For(x => x.MyLocationEnabled)
            .To(vm => vm.IsMyLocationEnabled);

        set.Apply();
        
        _googleMap.SetOnMarkerClickListener(this);
        _googleMap.SetOnMapClickListener(this);
        
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
        
        _googleMap.Clear();
            
        foreach (var place in ViewModel.Places)
        {
            Bitmap bitmap = null;

            switch (place.LocationType)
            {
                case LocationType.Event:
                {
                    var diameter = DimensUtils.DpToPx(Context, 32);
                    bitmap = PinUtils.CreateBitmap(diameter, DrawableUtils.CreateCircleDrawable(diameter, DimensUtils.DpToPx(Context, 4), Color.ParseColor(place.HexColor), Color.White));
                    
                    break;
                }
                case LocationType.School:
                {
                    var drawable = Resource.Drawable.ic_pin.GetDrawable(Context);
                    var diameter = Math.Min(drawable.IntrinsicWidth, drawable.IntrinsicHeight);
                    bitmap = PinUtils.CreateBitmap(diameter, DrawableUtils.CreateCircleDrawable(diameter, DimensUtils.DpToPx(Context, 4), Color.White, Color.ParseColor(place.HexColor)), drawable);
                    
                    break;
                }
            }
            
            _googleMap.AddMarker(new MarkerOptions()
                .SetIcon(BitmapDescriptorFactory.FromBitmap(bitmap))
                .SetSnippet(place.Id)
                .SetPosition(new LatLng(place.Latitude, place.Longitude))
                .SetTitle(place.Title));
        }
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
        if (ViewModel.SelectedLocation == null)
        {
            return;
        }
        
        ViewModel.SelectedLocation = null;
        _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
        _locationDetailsBottomSheetBehavior.PeekHeight = 0;
        _locationDetailsBottomSheetBehavior.Hideable = true;
        _locationDetailsBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
    }

    public bool OnMarkerClick(Marker marker)
    {
        SelectLocation(marker.Snippet);
        return true;
    }

    private void SelectLocation(string id)
    {
        ViewModel.SelectedLocation = ViewModel.Places?.FirstOrDefault(place => place.Id == id);

        if (ViewModel.SelectedLocation == null)
        {
            return;
        }
        
        _filtersBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
        var addressRect = new Rect();
        _addressLayout.GetDrawingRect(addressRect);
        _locationDetailsBottomSheet.OffsetDescendantRectToMyCoords(_addressLayout, addressRect);
        _locationDetailsBottomSheetBehavior.SetPeekHeight(addressRect.Top + addressRect.Height() + DimensUtils.DpToPx(Context, 56), false);
        _locationDetailsBottomSheetBehavior.Hideable = false;
        _locationDetailsBottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
    }

    public void OnMapClick(LatLng point) => DeselectLocation();
}

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
using Lct2023.Android.Adapters;
using Lct2023.Android.Decorations;
using Lct2023.Android.Definitions.Extensions;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using MvvmCross.Binding.ValueConverters;
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
    private const int DEFAULT_ZOOM = 8;
    
    private MapView _mapView;

    private GoogleMap _googleMap;
    private BottomSheetBehavior _bottomSheetBehavior;
    private View _parent;
    private MaterialCardView _bottomSheet;
    private View _addressLayout;

    protected override int GetLayoutId() => Resource.Layout.MapFragment;
    
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);
        
        view.FindViewById<TextView>(Resource.Id.title).Text = "Карта";
        _mapView = view.FindViewById<MapView>(Resource.Id.map_view);
        
        _parent = view.FindViewById(Resource.Id.map_container);
        
        var avatarImageButton = view.FindViewById<ImageView>(Resource.Id.toolbar_image);
        var filtersButton = view.FindViewById<MaterialButton>(Resource.Id.location_filters_button);
        var zoomInButton = view.FindViewById<MaterialButton>(Resource.Id.zoom_in_button);
        var zoomOutButton = view.FindViewById<MaterialButton>(Resource.Id.zoom_out_button);
        _bottomSheet = view.FindViewById<MaterialCardView>(Resource.Id.location_info_bottom_sheet);
        _bottomSheetBehavior = BottomSheetBehavior.From(_bottomSheet);
        
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

        var artDirectionsAdapter = new ArtDirectionAdapter((IMvxAndroidBindingContext)BindingContext)
        {
            ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.ArtDirectionItem),
        };

        var horizontal16dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 16), LinearLayoutManager.Horizontal);
        var horizontal8dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(Activity, 8), LinearLayoutManager.Horizontal);
        
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
        
        _bottomSheetBehavior.Hideable = true;
        _bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;

        foreach (var button in new [] { filtersButton, closeBsButton, zoomInButton, zoomOutButton, })
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
            .To(vm => vm.SelectedLocation.ArtDirections);
        
        set
            .Bind(contactsItemsLayout)
            .For(v => v.ItemsSource)
            .To(vm => vm.Contacts);
        
        set
            .Bind(artDirectionsLayout)
            .For(v => v.BindVisible())
            .To(vm => vm.SelectedLocation.ArtDirections)
            .WithConversion(new AnyExpressionConverter<IEnumerable<ArtDirectionType>, bool>(items => items?.Any() == true));

        set
            .Bind(view.FindViewById<MaterialButton>(Resource.Id.open_address_button))
            .For(v => v.BindClick())
            .To(vm => vm.OpenMapCommand);
        
        var anyEventsVisibilityConverter = new AnyExpressionConverter<IEnumerable<EventItemViewModel>, bool>(items => items?.Any() == true);
        
        set
            .Bind(view.FindViewById(Resource.Id.events_header_text_view))
            .For(v => v.BindVisible())
            .To(vm => vm.Events)
            .WithConversion(anyEventsVisibilityConverter);
        
        set
            .Bind(afishaLayout)
            .For(v => v.BindVisible())
            .To(vm => vm.Events)
            .WithConversion(anyEventsVisibilityConverter);
        
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

        set.Apply();

        this.WhenAnyValue(v => v.ViewModel.MyPosition)
            .WhereNotNull()
            .Subscribe(_ =>
            {
                try
                {
                    _googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(ViewModel.MyPosition.Latitude, ViewModel.MyPosition.Longitude), DEFAULT_ZOOM));
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
        if (_googleMap == null)
        {
            return;
        }
        
        _googleMap.Clear();
            
        foreach (var place in ViewModel.Places)
        {
            _googleMap.AddMarker(new MarkerOptions()
                .SetIcon(BitmapDescriptorFactory.FromBitmap(Resource.Drawable.ic_pin.GetDrawable(Context).AddCircleWithStroke(Context.ToPixels(2), Color.White, Resource.Color.pin_green_color, Context)))
                .SetSnippet(place.Id)
                .SetPosition(new LatLng(place.Latitude, place.Longitude))
                .SetTitle(place.Title));
        }
    }

    public void OnClick(View view)
    {
        switch (view.Id)
        {
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
        _bottomSheetBehavior.PeekHeight = 0;
        _bottomSheetBehavior.Hideable = true;
        _bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
    }

    public bool OnMarkerClick(Marker marker)
    {
        ViewModel.SelectedLocation = ViewModel.Places?.FirstOrDefault(place => place.Id == marker.Snippet);
        _parent.Post(() =>
        {
            var addressRect = new Rect();
            _addressLayout.GetDrawingRect(addressRect);
            _bottomSheet.OffsetDescendantRectToMyCoords(_addressLayout, addressRect);
            _bottomSheetBehavior.SetPeekHeight(addressRect.Top + addressRect.Height() + Context.ToPixels(56), false);
            _bottomSheetBehavior.Hideable = false;
            _bottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
        });

        return true;
    }

    public void OnMapClick(LatLng point) => DeselectLocation();
}

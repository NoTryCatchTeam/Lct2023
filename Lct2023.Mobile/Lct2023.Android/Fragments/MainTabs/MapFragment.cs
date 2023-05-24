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
using Android.Widget;
using DynamicData.Binding;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Lct2023.Converters;
using Lct2023.Definitions.Enums;
using MvvmCross.Binding.ValueConverters;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.Views;
using ReactiveUI;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class MapFragment : BaseFragment<MapViewModel>, IOnMapReadyCallback, View.IOnClickListener
{
    private const int DEFAULT_ZOOM = 8;
    
    private MapView _mapView;

    private GoogleMap _googleMap;
    private BottomSheetBehavior _bottomSheetBehaviour;

    protected override int GetLayoutId() => Resource.Layout.MapFragment;
    
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);
        
        _mapView = view.FindViewById<MapView>(Resource.Id.map_view);
        
        var zoomInButton = view.FindViewById<MaterialButton>(Resource.Id.zoom_in_button);
        var zoomOutButton = view.FindViewById<MaterialButton>(Resource.Id.zoom_out_button);
        var bottomSheet = view.FindViewById<MaterialCardView>(Resource.Id.location_info_bottom_sheet);
        _bottomSheetBehaviour = BottomSheetBehavior.From(bottomSheet);
        
        var closeBsButton = view.FindViewById<MaterialButton>(Resource.Id.close_bs_button);
        var contactsItemsLayout = view.FindViewById<MvxLinearLayout>(Resource.Id.contacts_items_layout);
        var moreContactsButton = view.FindViewById<MaterialButton>(Resource.Id.more_contacts_button);
        
        var addressTextView = view.FindViewById<TextView>(Resource.Id.address_text_view);
        var mailTextView = view.FindViewById<TextView>(Resource.Id.mail_text_view);
        var siteTextView = view.FindViewById<TextView>(Resource.Id.site_text_view);
        
        var socialLinksLayout = view.FindViewById<MvxLinearLayout>(Resource.Id.open_social_nets_items_layout);
        var artDirectionsLayout = view.FindViewById<MvxRecyclerView>(Resource.Id.art_directions_layout);
        var afishaLayout = view.FindViewById<MvxRecyclerView>(Resource.Id.afisha_layout);

        socialLinksLayout.ItemTemplateId = Resource.Layout.OpenSocialNetItem;
        artDirectionsLayout.ItemTemplateId = Resource.Layout.ArtDirectionItem;
        contactsItemsLayout.ItemTemplateId = Resource.Layout.ContactItem;
        afishaLayout.ItemTemplateId = Resource.Layout.AfishItem;
        
        _bottomSheetBehaviour.Hideable = true;
        _bottomSheetBehaviour.State = BottomSheetBehavior.StateHidden;

        foreach (var button in new [] { closeBsButton, zoomInButton, zoomOutButton, })
        {
            button.SetOnClickListener(this);
        }
        
        _mapView?.OnCreate(savedInstanceState);
        _mapView?.GetMapAsync(this);
        
        ViewModel.Places
            .ObserveCollectionChanges()
            .Do(_ => _googleMap?.Clear())
            .Where(_ => _googleMap != null)
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
            .For(v => v.BindClick())
            .To(vm => vm.CopyCommand)
            .WithConversion<MvxCommandParameterValueConverter>(mailTextView.Text);
        
        set
            .Bind(addressTextView)
            .For(v => v.BindClick())
            .To(vm => vm.CopyCommand)
            .WithConversion<MvxCommandParameterValueConverter>(addressTextView.Text);
        
        set
            .Bind(siteTextView)
            .For(v => v.BindClick())
            .To(vm => vm.CopyCommand)
            .WithConversion<MvxCommandParameterValueConverter>(siteTextView.Text);
        
        set
            .Bind(view.FindViewById<MaterialButton>(Resource.Id.gps_button))
            .For(v => v.BindClick())
            .To(vm => vm.GpsCommand);
        
        set
            .Bind(view.FindViewById<MapView>(Resource.Id.location_filters_button))
            .For(v => v.BindClick())
            .To(vm => vm.FiltersCommand);

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
            .To(vm => vm.SelectedLocation.SocialLinks);
        
        set
            .Bind(artDirectionsLayout)
            .For(v => v.ItemsSource)
            .To(vm => vm.SelectedLocation.ArtDirections);
        
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
            .To(vm => vm.SelectedLocation.Events)
            .WithConversion(anyEventsVisibilityConverter);
        
        set
            .Bind(afishaLayout)
            .For(v => v.BindVisible())
            .To(vm => vm.SelectedLocation.Events)
            .WithConversion(anyEventsVisibilityConverter);
        
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
            .WithConversion(new AnyExpressionConverter<IEnumerable<string>, bool>(items => items?.Count() > 1));


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
        
        _googleMap.MapType = GoogleMap.MapTypeNormal;
        _googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(55.7499931, 37.624216), 10));

        _googleMap.UiSettings.CompassEnabled = false;
        _googleMap.UiSettings.ZoomControlsEnabled = false;
        _googleMap.UiSettings.MyLocationButtonEnabled = false;
        
        UpdateMarkers();
    }

    private void UpdateMarkers()
    {
        foreach (var place in ViewModel.Places)
        {
            _googleMap.AddMarker(new MarkerOptions()
                .SetPosition(new LatLng(place.Latitude, place.Longitude))
                .SetTitle(place.Title));
        }
    }

    public void OnClick(View view)
    {
        switch (view.Id)
        {
            case Resource.Id.close_bs_button:
                ViewModel.SelectedLocation = null;
                _bottomSheetBehaviour.PeekHeight = 0;
                _bottomSheetBehaviour.Hideable = true;
                _bottomSheetBehaviour.State = BottomSheetBehavior.StateHidden;
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
}

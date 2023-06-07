using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils.Clustering;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.RecyclerView.Widget;
using DataModel.Definitions.Enums;
using DynamicData.Binding;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Google.Android.Material.TextField;
using Lct2023.Android.Adapters;
using Lct2023.Android.Callbacks;
using Lct2023.Android.Decorations;
using Lct2023.Android.Definitions.Extensions;
using Lct2023.Android.Definitions.Models.Map;
using Lct2023.Android.Helpers;
using Lct2023.Android.Listeners;
using Lct2023.Converters;
using Lct2023.ViewModels.Map;
using MvvmCross.Binding.ValueConverters;
using MvvmCross.DroidX.RecyclerView;
using MvvmCross.DroidX.RecyclerView.ItemTemplates;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Views;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using ReactiveUI;

namespace Lct2023.Android.Activities.Map
{
    [MvxActivityPresentation]
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class LocationsMapActivity : BaseActivity<LocationsMapViewModel>, IOnMapReadyCallback, View.IOnClickListener, GoogleMap.IOnMarkerClickListener, GoogleMap.IOnMapClickListener, ClusterManager.IOnClusterItemClickListener
    {
        private const float MAX_DIM_ALPHA = 0.5f;

        private MapView _mapView;

        private GoogleMap _googleMap;
        private ClusterManager _clusterManager;
        private BottomSheetBehavior _locationDetailsBottomSheetBehavior;
        private MaterialCardView _locationDetailsBottomSheet;
        private View _addressLayout;
        private CoordinatorLayout _mapContainer;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LocationsMapActivity);

            Toolbar.Title = "Карта";

            _mapView = FindViewById<MapView>(Resource.Id.locations_map_view);
            _mapContainer = FindViewById<CoordinatorLayout>(Resource.Id.locations_map_container);
            var locationTypesGroup = FindViewById<MaterialButtonToggleGroup>(Resource.Id.location_types_group);
            var zoomInButton = FindViewById<MaterialButton>(Resource.Id.zoom_in_button);
            var zoomOutButton = FindViewById<MaterialButton>(Resource.Id.zoom_out_button);
            _locationDetailsBottomSheet = FindViewById<MaterialCardView>(Resource.Id.locations_map_info_bottom_sheet);
            _locationDetailsBottomSheetBehavior = BottomSheetBehavior.From(_locationDetailsBottomSheet);

            var closeBsButton = FindViewById<MaterialButton>(Resource.Id.close_bs_button);
            var contactsItemsLayout = FindViewById<MvxLinearLayout>(Resource.Id.contacts_items_layout);
            var moreContactsButton = FindViewById<MaterialButton>(Resource.Id.more_contacts_button);

            _addressLayout = FindViewById(Resource.Id.location_address_layout);
            var addressTextView = FindViewById<TextView>(Resource.Id.address_text_view);
            var mailTextView = FindViewById<TextView>(Resource.Id.mail_text_view);
            var siteTextView = FindViewById<TextView>(Resource.Id.site_text_view);

            var socialLinksLayout = FindViewById<MvxRecyclerView>(Resource.Id.open_social_nets_items_layout);
            var artDirectionsLayout = FindViewById<MvxRecyclerView>(Resource.Id.art_directions_layout);
            var afishaLayout = FindViewById<MvxRecyclerView>(Resource.Id.afisha_layout);
            var actionButton = FindViewById<MaterialButton>(Resource.Id.action_button);
            var dimView = FindViewById(Resource.Id.locations_map_dim);

            var artDirectionsAdapter = new StreamsAdapter((IMvxAndroidBindingContext)BindingContext)
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.ArtDirectionItem),
            };

            var horizontal16dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(this, 16), LinearLayoutManager.Horizontal);
            var horizontal8dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(this, 8), LinearLayoutManager.Horizontal);
            var vertical16dpItemSpacingDecoration = new ItemSeparateDecoration(DimensUtils.DpToPx(this, 16), LinearLayoutManager.Vertical);

            artDirectionsLayout.SetLayoutManager(new MvxGuardedLinearLayoutManager(this) { Orientation = LinearLayoutManager.Horizontal });
            artDirectionsLayout.SetAdapter(artDirectionsAdapter);
            artDirectionsLayout.AddItemDecoration(horizontal8dpItemSpacingDecoration);

            contactsItemsLayout.ItemTemplateId = Resource.Layout.ContactItem;

            var afishaAdapter = new EventsAdapter((IMvxAndroidBindingContext)BindingContext)
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.AfishItem),
            };
            afishaLayout.SetLayoutManager(new MvxGuardedLinearLayoutManager(this) { Orientation = LinearLayoutManager.Horizontal });
            afishaLayout.AddItemDecoration(horizontal8dpItemSpacingDecoration);
            afishaLayout.SetAdapter(afishaAdapter);

            var socialLinksAdapter = new SocialLinksAdapter((IMvxAndroidBindingContext)BindingContext)
            {
                ItemTemplateSelector = new MvxDefaultTemplateSelector(Resource.Layout.SocialLinktem),
            };
            socialLinksLayout.ItemTemplateId = Resource.Layout.SocialLinktem;
            socialLinksLayout.SetLayoutManager(new MvxGuardedLinearLayoutManager(this) { Orientation = LinearLayoutManager.Horizontal });
            socialLinksLayout.AddItemDecoration(horizontal16dpItemSpacingDecoration);
            socialLinksLayout.SetAdapter(socialLinksAdapter);

            dimView.SetOnTouchListener(new DefaultOnTouchListener((v, e) =>
            {
                var isExpanded = _locationDetailsBottomSheetBehavior.State == BottomSheetBehavior.StateExpanded;
                if (isExpanded || _locationDetailsBottomSheetBehavior.State == BottomSheetBehavior.StateCollapsed)
                {
                    DeselectLocation();
                }

                return isExpanded;
            }));

            var bottomSheetCallback = new DefaultBottomSheetCallback(
                (v, s) => dimView.Alpha = Math.Min(s, MAX_DIM_ALPHA));

            _locationDetailsBottomSheetBehavior.AddBottomSheetCallback(bottomSheetCallback);
            _locationDetailsBottomSheetBehavior.Hideable = true;
            _locationDetailsBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;

            foreach (var button in new[] { closeBsButton, zoomInButton, zoomOutButton })
            {
                button.SetOnClickListener(this);
            }

            _mapView?.OnCreate(bundle);
            _mapView?.GetMapAsync(this);

            ViewModel.Places
                .ObserveCollectionChanges()
                .Throttle(TimeSpan.FromMilliseconds(150), RxApp.MainThreadScheduler)
                .Subscribe(_ => UpdateMarkers())
                .DisposeWith(CompositeDisposable);

            var set = CreateBindingSet();

            set
                .Bind(FindViewById<TextView>(Resource.Id.location_title))
                .For(v => v.Text)
                .To(vm => vm.SelectedLocation.Title);

            set
                .Bind(FindViewById<TextView>(Resource.Id.location_description))
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
                .Bind(FindViewById<MaterialButton>(Resource.Id.gps_button))
                .For(v => v.BindClick())
                .To(vm => vm.GpsCommand);

            var visibilityConverter = new AnyExpressionConverter<string, bool>(text => !string.IsNullOrEmpty(text));

            set
                .Bind(FindViewById(Resource.Id.site_layout))
                .For(v => v.BindVisible())
                .To(vm => vm.SelectedLocation.Site)
                .WithConversion(visibilityConverter);

            set
                .Bind(FindViewById(Resource.Id.site_bottom_line))
                .For(v => v.BindVisible())
                .To(vm => vm.SelectedLocation.Site)
                .WithConversion(visibilityConverter);

            var itemsVisibilityConverter = new AnyExpressionConverter<IEnumerable<object>, bool>(items => items?.Any() == true);

            set
                .Bind(FindViewById(Resource.Id.contacts_layout))
                .For(v => v.BindVisible())
                .To(vm => vm.SelectedLocation.Contacts)
                .WithConversion(itemsVisibilityConverter);

            set
                .Bind(FindViewById(Resource.Id.contacts_bottom_line))
                .For(v => v.BindVisible())
                .To(vm => vm.SelectedLocation.Contacts)
                .WithConversion(itemsVisibilityConverter);

            set
                .Bind(FindViewById(Resource.Id.mail_layout))
                .For(v => v.BindVisible())
                .To(vm => vm.SelectedLocation.Email)
                .WithConversion(visibilityConverter);

            set
                .Bind(FindViewById(Resource.Id.mail_bottom_line))
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
                .Bind(artDirectionsLayout)
                .For(v => v.BindVisible())
                .To(vm => vm.SelectedLocation.Streams)
                .WithConversion(itemsVisibilityConverter);

            set
                .Bind(FindViewById<MaterialButton>(Resource.Id.open_address_button))
                .For(v => v.BindClick())
                .To(vm => vm.OpenMapCommand);

            set
                .Bind(FindViewById(Resource.Id.events_header_text_view))
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
                        _googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(ViewModel.MyPosition.Latitude, ViewModel.MyPosition.Longitude), 14));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                })
                .DisposeWith(CompositeDisposable);
        }

        protected override void OnStart()
        {
            base.OnStart();
            _mapView?.OnStart();
        }

        protected override void OnResume()
        {
            base.OnResume();
            _mapView?.OnResume();
        }

        protected override void OnPause()
        {
            _mapView?.OnPause();
            base.OnPause();
        }

        protected override void OnStop()
        {
            _mapView?.OnStop();
            base.OnStop();
        }

        protected override void OnDestroy()
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

            _clusterManager = new ClusterManager(this, googleMap);
            _clusterManager.Renderer = new MapClusterRenderer(this, googleMap, _clusterManager);

            var set = CreateBindingSet();

            set.Bind(_googleMap)
                .For(x => x.MyLocationEnabled)
                .To(vm => vm.IsMyLocationEnabled);

            set.Apply();

            _googleMap.SetOnCameraIdleListener(_clusterManager);
            _googleMap.SetOnMarkerClickListener(this);
            _googleMap.SetOnMapClickListener(this);
            _clusterManager.SetOnClusterItemClickListener(this);

            _googleMap.MapType = GoogleMap.MapTypeNormal;
            _googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(55.7499931, 37.624216), 9));

            _googleMap.UiSettings.CompassEnabled = false;
            _googleMap.UiSettings.ZoomControlsEnabled = false;
            _googleMap.UiSettings.MyLocationButtonEnabled = false;

            UpdateMarkers();
        }

        private void UpdateMarkers()
        {
            KeyboardUtils.HideKeyboard(this);
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
            _locationDetailsBottomSheetBehavior.PeekHeight = 0;
            _locationDetailsBottomSheetBehavior.Hideable = true;
            _locationDetailsBottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
        }

        public bool OnClusterItemClick(Java.Lang.Object item)
        {
            if (item is MapClusterItem clusterItem)
            {
                SelectLocation(clusterItem.Snippet);
            }

            return false;
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

            _mapContainer.Post(() =>
            {
                var addressRect = new Rect();
                _addressLayout.GetDrawingRect(addressRect);
                _locationDetailsBottomSheet.OffsetDescendantRectToMyCoords(_addressLayout, addressRect);
                _locationDetailsBottomSheetBehavior.SetPeekHeight(addressRect.Top + addressRect.Height() + DimensUtils.DpToPx(this, 32), false);
                _locationDetailsBottomSheetBehavior.Hideable = false;
                _locationDetailsBottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
            });
        }

        public void OnMapClick(LatLng point) => DeselectLocation();
    }
}


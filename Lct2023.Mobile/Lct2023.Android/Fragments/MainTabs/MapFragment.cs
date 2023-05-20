using System.Reactive.Disposables;
using Android.Gms.Maps;
using Android.OS;
using Android.Views;
using Lct2023.ViewModels.Map;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using System;
using System.Linq;
using System.Reactive.Linq;
using Android.Gms.Maps.Model;
using DynamicData.Binding;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class MapFragment : BaseFragment<MapViewModel>, IOnMapReadyCallback
{
    private MapView _mapView;

    private GoogleMap _googleMap;
    
    protected override int GetLayoutId() => Resource.Layout.MapFragment;
    
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);
        
        _mapView = view.FindViewById<MapView>(Resource.Id.map_view);
        _mapView?.OnCreate(savedInstanceState);
        _mapView?.GetMapAsync(this);
        
        ViewModel.Places
            .ObserveCollectionChanges()
            .Subscribe(_ => UpdateMarkers())
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
        _googleMap.MapType = GoogleMap.MapTypeNormal;
        _googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(55.7499931, 37.624216), 10));

        UpdateMarkers();
    }

    private void UpdateMarkers()
    {
        _googleMap?.Clear();
        
        foreach (var place in ViewModel.Places)
        {
            _googleMap?.AddMarker(new MarkerOptions()
                .SetPosition(new LatLng(place.Latitude, place.Longitude))
                .SetTitle(place.Name));
        }
    }
}

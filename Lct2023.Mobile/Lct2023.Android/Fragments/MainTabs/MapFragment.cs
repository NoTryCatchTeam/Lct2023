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
using ReactiveUI;

namespace Lct2023.Android.Fragments.MainTabs;

[MvxFragmentPresentation]
public class MapFragment : BaseFragment<MapViewModel>, IOnMapReadyCallback
{
    private MapView _mapView;
    protected override int GetLayoutId() => Resource.Layout.MapFragment;
    
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = base.OnCreateView(inflater, container, savedInstanceState);
        
        _mapView = view.FindViewById<MapView>(Resource.Id.map_view);
        _mapView?.OnCreate(savedInstanceState);
        _mapView?.GetMapAsync(this);

        this.WhenAnyValue(a => a.ViewModel.Schools, a => a.GoogleMap)
            .Do(_ => GoogleMap?.Clear())
            .Where(changeSet
                => changeSet.Item1?.Any() == true
                   && changeSet.Item2 != null)
            .Subscribe(_ => UpdateMarkers())
            .DisposeWith(CompositeDisposable);

        return view;
    }
    
    public GoogleMap GoogleMap { get; private set; }
    
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
        GoogleMap = googleMap;
        GoogleMap.MapType = GoogleMap.MapTypeNormal;
        GoogleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(55.7499931, 37.624216), 10));
    }

    private void UpdateMarkers()
    {
        foreach (var school in ViewModel.Schools)
        {
            GoogleMap.AddMarker(new MarkerOptions()
                .SetPosition(new LatLng(school.Latitude, school.Longitude))
                .SetTitle(school.Name));
        }
    }
}

using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Lct2023.ViewModels.Map;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using ReactiveUI;

namespace Lct2023.Android.Activities;

[MvxActivityPresentation]
[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
public class MapActivity : BaseActivity<MapViewModel>, IOnMapReadyCallback
{
    private SupportMapFragment _mapFragment;

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.map_activity);
        _mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map_fragment);
        _mapFragment?.GetMapAsync(this);

        this.WhenAnyValue(a => a.ViewModel.Schools, a => a.GoogleMap)
            .Do(_ => GoogleMap?.Clear())
            .Where(changeSet
                => changeSet.Item1?.Any() == true
                   && changeSet.Item2 != null)
            .Subscribe(_ => UpdateMarkers())
            .DisposeWith(CompositeDisposable);
    }
    
    public GoogleMap GoogleMap { get; private set; }

    private void UpdateMarkers()
    {
        foreach (var school in ViewModel.Schools)
        {
            GoogleMap.AddMarker(new MarkerOptions()
                .SetPosition(new LatLng(school.Latitude, school.Longitude))
                .SetTitle(school.Name));
        }
    }

    public void OnMapReady(GoogleMap googleMap)
    {
        GoogleMap = googleMap;
        GoogleMap.MapType = GoogleMap.MapTypeNormal;
        GoogleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(55.7499931, 37.624216), 10));
    }
}

using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Lct2023.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Lct2023.Android.Activities;

[MvxActivityPresentation]
[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
public class MapActivity : MvxActivity<MapViewModel>, IOnMapReadyCallback
{
    private SupportMapFragment _mapFragment;

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.map_activity);
        _mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map_fragment);
        _mapFragment?.GetMapAsync(this);
    }
    
    public void OnMapReady(GoogleMap googleMap)
    {
        googleMap.MapType = GoogleMap.MapTypeNormal;
        googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(55.7499931, 37.624216), 10));
    }
}

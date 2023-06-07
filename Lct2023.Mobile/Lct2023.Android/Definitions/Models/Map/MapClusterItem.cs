using System;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils.Clustering;
using DataModel.Definitions.Enums;
using Lct2023.ViewModels.Map;

namespace Lct2023.Android.Definitions.Models.Map
{
    public class MapClusterItem : Java.Lang.Object, IClusterItem
    {
        public MapClusterItem(PlaceItemViewModel place)
        {
            Position = new LatLng(place.Latitude, place.Longitude);
            Snippet = place.Id;
            Title = place.Title;
            LocationType = place.LocationType;
            HexColor = place.HexColor;
        }

        public LatLng Position { get; }

        public string Snippet { get; }

        public string Title { get; }

        public LocationType LocationType { get; }

        public string HexColor { get; }
    }
}


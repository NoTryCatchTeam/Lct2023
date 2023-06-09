﻿using System;
using System.Linq;
using System.Collections;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils.Clustering;
using Android.Gms.Maps.Utils.Clustering.View;
using Android.Graphics;
using DataModel.Definitions.Enums;
using Lct2023.Android.Definitions.Extensions;
using Lct2023.Android.Helpers;
using System.Collections.Generic;
using DynamicData;
using Lct2023.Commons.Extensions;

namespace Lct2023.Android.Definitions.Models.Map
{
    public class MapClusterRenderer : DefaultClusterRenderer
    {
        private const string DEFAULT_PIN_HEX_COLOR = "#8fb14c";

        private readonly Color _defaultColor = Color.ParseColor(DEFAULT_PIN_HEX_COLOR);
        private readonly Context _context;

        public MapClusterRenderer(Context context, GoogleMap map, ClusterManager clusterManager)
            : base(context, map, clusterManager)
        {
            _context = context;
        }

        protected override void OnBeforeClusterRendered(ICluster cluster, MarkerOptions markerOptions)
        {
            base.OnBeforeClusterRendered(cluster, markerOptions);

            GetClusterBitmapDescriptor(cluster)?.Then(bitmapDes => markerOptions.SetIcon(bitmapDes));
        }

        protected override void OnClusterRendered(ICluster cluster, Marker marker)
        {
            base.OnClusterRendered(cluster, marker);

            GetClusterBitmapDescriptor(cluster)?.Then(bitmapDes => marker.SetIcon(bitmapDes));
        }

        protected override void OnBeforeClusterItemRendered(Java.Lang.Object item, MarkerOptions markerOptions)
        {
            base.OnBeforeClusterItemRendered(item, markerOptions);

            // Its always MapClusterItem, but need to cast anyway
            if (item is not MapClusterItem clusterItem)
            {
                return;
            }

            Bitmap bitmap = null;

            switch (clusterItem.LocationType)
            {
                case LocationType.Event:
                {
                    var diameter = DimensUtils.DpToPx(_context, 32);
                    bitmap = PinUtils.CreateBitmap(diameter, DrawableUtils.CreateCircleDrawable(diameter, DimensUtils.DpToPx(_context, 4), Color.ParseColor(clusterItem.HexColor), Color.White));

                    break;
                }
                case LocationType.School:
                {
                    var drawable = Resource.Drawable.ic_pin.GetDrawable(_context);
                    var diameter = Math.Min(drawable.IntrinsicWidth, drawable.IntrinsicHeight);
                    bitmap = PinUtils.CreateBitmap(diameter, DrawableUtils.CreateCircleDrawable(diameter, DimensUtils.DpToPx(_context, 4), Color.White, Color.ParseColor(clusterItem.HexColor)),
                        drawable);

                    break;
                }
            }

            markerOptions.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmap));
        }

        private BitmapDescriptor GetClusterBitmapDescriptor(ICluster cluster)
        {
            if (cluster.Items.Count <= 0)
            {
                return null;
            }

            LocationType locationType = LocationType.School;
            var colors = new List<string>();


            foreach (MapClusterItem clusterItem in cluster.Items)
            {
                locationType = clusterItem.LocationType;
                if (locationType == LocationType.Event)
                {
                    break;
                }
                colors.Add(clusterItem.HexColor);
            }

            var textSize = DimensUtils.DpToPx(_context, 14);
            var diameter = DimensUtils.DpToPx(_context, 32);

            switch (locationType)
            {
                case LocationType.Event:
                    return BitmapDescriptorFactory.FromBitmap(PinUtils.CreateBitmapWithText(diameter, $"{cluster.Size}", _defaultColor, textSize, DrawableUtils.CreateCircleDrawable(diameter, DimensUtils.DpToPx(_context, 4), _defaultColor, Color.White)));
                default:
                    return BitmapDescriptorFactory.FromBitmap(PinUtils.CreateBitmapWithText(diameter, $"{cluster.Size}", Color.White, textSize, DrawableUtils.CreateCircleDrawable(diameter, DimensUtils.DpToPx(_context, 4), Color.White, Color.ParseColor(colors.GroupBy(c => c).OrderByDescending(g => g.Count()).First().Key))));
            }
        }
    }
}

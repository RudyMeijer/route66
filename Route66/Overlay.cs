using GMap.NET.WindowsForms;
using System.Drawing;
using System;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.ObjectModel;

namespace Route66
{
    internal class Overlay
    {
        private GMapControl gmap;
        private GMapOverlay mOverlay;
        private GMapRoute mRoute;
        private GMapMarker mCurrentMarker;

        public Overlay(GMapControl gmap, string markers)
        {
            this.gmap = gmap;
            mOverlay = new GMapOverlay(markers);
            mRoute = new GMapRoute("routes");
            mRoute.Stroke = new Pen(Color.Red, 2);
            mOverlay.Routes.Add(mRoute);
            gmap.Overlays.Add(mOverlay);
        }

        public void AddMarker(int x, int y)
        {
            PointLatLng point = gmap.FromLocalToLatLng(x,y);
            var marker = new GMarkerGoogle(point, GMarkerGoogleType.red_small);

            var idx = mOverlay.Markers.IndexOf(mCurrentMarker)+1;
            mOverlay.Markers.Insert(idx, marker);
            mCurrentMarker = marker;
            mOverlay.Routes[0].Points.Add(point);
            UpdateRoute();
            Console.WriteLine($"Marker {marker.Tag} added at {marker.LocalPosition}");
        }
        public void RemoveCurrentMarker()
        {
            mOverlay.Markers.Remove(mCurrentMarker);
            UpdateRoute();
            mCurrentMarker = null;
        }
        public void UpdateRoute()
        {
            var id = 0;
            mRoute.Points.Clear();
            foreach (var item in mOverlay.Markers)
            {
                item.Tag = id++;
                mRoute.Points.Add(item.Position);
            }
            gmap.UpdateRouteLocalPosition(mRoute);
        }
        public void SetTooltipOnOff(bool on)
        {
            foreach (var item in mOverlay.Markers)
            {
                item.ToolTipMode = (on) ? MarkerTooltipMode.OnMouseOver : MarkerTooltipMode.Never;
                item.ToolTipText = $"id {item.Tag} Dosing {item.LocalPosition.X} gr.\nLat={item.Position.Lat:f3} Lng={item.Position.Lng:f3}";
            }
        }
        internal void Clear()
        {
            mOverlay.Markers.Clear();
            mRoute.Clear();
            gmap.UpdateRouteLocalPosition(mRoute);
            mCurrentMarker = null;
        }

        internal void SetCurrentMarker(GMapMarker item)
        {
            mCurrentMarker = item;
        }

        internal void UpdateCurrentMarkerPosition(int x, int y)
        {
            if (mCurrentMarker == null) return;
            mCurrentMarker.Position = gmap.FromLocalToLatLng(x, y);
            UpdateRoute();
        }

        internal void Load(Route route)
        {
            Clear();
            foreach (var item in route.GpsMarkers)
            {
                mOverlay.Markers.Add(new GMarkerGoogle(new PointLatLng(item.Lat, item.Lng), GMarkerGoogleType.red_small));
            }
            UpdateRoute();
            gmap.ZoomAndCenterRoute(mOverlay.Routes[0]);
        }

        internal void Save(Route route)
        {
            route.GpsMarkers.Clear();
            foreach (var item in mOverlay.Markers)
            {
                route.GpsMarkers.Add(new GpsMarker(item.Position.Lng, item.Position.Lat));
            }
        }
    }
}
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
        private GMarkerGoogle head;

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
            if (mCurrentMarker == head)
            {
                mOverlay.Markers.Add(marker);
                head = marker;
            }
            else // Insert marker.
            {
                var idx = mOverlay.Markers.IndexOf(mCurrentMarker);
                mOverlay.Markers.Insert(idx, marker);
            }
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
            mCurrentMarker = head = null;
        }

        internal void SetCurrentMarker(GMapMarker item)
        {
            mCurrentMarker = item;
        }

        internal void UpdateCurrentMarker(int x, int y)
        {
            if (mCurrentMarker == null) return;
            mCurrentMarker.Position = gmap.FromLocalToLatLng(x, y);
            UpdateRoute();
        }
    }
}
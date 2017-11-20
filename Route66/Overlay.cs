using GMap.NET.WindowsForms;
using System.Drawing;
using System;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.ObjectModel;
using System.Windows.Forms;
namespace Route66
{
    internal class Overlay
    {
        private GMapControl Map;
        private GMapMarker CurrentMarker;
        private readonly GMapRoute RedRoute;
        private readonly GMapOverlay Red;
        private readonly GMapOverlay Green;
        private readonly GMapOverlay Blue;

        public Overlay(GMapControl gmap)
        {
            this.Map = gmap;
            gmap.Overlays.Add(new GMapOverlay("Gps points"));
            gmap.Overlays.Add(new GMapOverlay("Change points"));
            gmap.Overlays.Add(new GMapOverlay("Navigation points"));
            Red = gmap.Overlays[0];
            Green = gmap.Overlays[1];
            Blue = gmap.Overlays[2];
            RedRoute = new GMapRoute("routes")
            {
                Stroke = new Pen(Color.Red, 2)
            };
            Red.Routes.Add(RedRoute);
        }

        public void AddMarker(int x, int y)
        {
            PointLatLng point = Map.FromLocalToLatLng(x, y);
            var marker = new GMarkerGoogle(point, GMarkerGoogleType.red_small);

            var idx = Red.Markers.IndexOf(CurrentMarker) + 1;
            CurrentMarker = marker;
            marker.ToolTipText = idx.ToString();
            Red.Markers.Insert(idx, marker);
            RedRoute.Points.Insert(idx,point);
            Map.UpdateRouteLocalPosition(RedRoute);
            Console.WriteLine($"Marker added at {marker.LocalPosition}");
        }
        public void RemoveCurrentMarker()
        {
            if (CurrentMarker == null) return;
            Red.Markers.Remove(CurrentMarker);
            UpdateGreenAndBlueOverlay(Crud.Delete, CurrentMarker.Tag, null);
            Map.UpdateMarkerLocalPosition(CurrentMarker);
            RedRoute.Points.Remove(CurrentMarker.Position);
            Map.UpdateRouteLocalPosition(RedRoute);
            CurrentMarker = null;
        }
        public void UpdateCurrentMarkerPosition(int x, int y)
        {
            if (CurrentMarker == null) return;
            var newPosition = Map.FromLocalToLatLng(x, y);
            if (CurrentMarker.Tag is ChangeMarker) foreach (var item in Green.Markers) if (item.Position == CurrentMarker.Position)
                        item.Position = newPosition;
            if (CurrentMarker.Tag is NavigationMarker) foreach (var item in Blue.Markers) if (item.Position == CurrentMarker.Position)
                        item.Position = newPosition;
            // 
            // Update Route point.
            //
            var idx = RedRoute.Points.IndexOf(CurrentMarker.Position);
            RedRoute.Points[idx] = newPosition;
            Map.UpdateRouteLocalPosition(RedRoute);
            //
            // Update current marker position.
            //
            foreach (var item in Red.Markers) if (item == CurrentMarker) item.Position = newPosition;
            CurrentMarker.Position = newPosition;
        }
        public void UpdateRoute()
        {
            RedRoute.Points.Clear();
            foreach (var item in Red.Markers)
            {
                RedRoute.Points.Add(item.Position);
            }
            //UpdateGreenAndBlueOverlay();
            Map.UpdateRouteLocalPosition(RedRoute);
        }
        public void SetTooltipOnOff(bool on)
        {
            foreach (var item in Red.Markers)
            {
                //item.ToolTipMode = (on) ? MarkerTooltipMode.OnMouseOver : MarkerTooltipMode.Never;
                if (item.Tag == null) item.ToolTipText = $"Lat={item.Position.Lat:f3} Lng={item.Position.Lng:f3}";
            }
        }
        public void Clear()
        {
            Red.Markers.Clear();
            Green.Markers.Clear();
            Blue.Markers.Clear();
            RedRoute.Clear();
            Map.UpdateRouteLocalPosition(RedRoute);
            CurrentMarker = null;
        }

        public void SetCurrentMarker(GMapMarker item)
        {
            CurrentMarker = item;
        }

        /// <summary>
        /// Copy Route class into overlays.
        /// </summary>
        /// <param name="route"></param>
        public void Load(Route route)
        {
            Red.Markers.Clear();
            Green.Markers.Clear();
            Blue.Markers.Clear();
            foreach (var item in route.GpsMarkers)
            {
                Red.Markers.Add(new GMarkerGoogle(new PointLatLng(item.Lat, item.Lng), GMarkerGoogleType.red_small));
                //
                // Copy green and blue tags into red markers.
                //
                var currentmarker = Red.Markers[Red.Markers.Count - 1];
                //var pos = red.Position;
                foreach (var green in route.ChangeMarkers) if (green.Lat == item.Lat && green.Lng == item.Lng)
                    {
                        currentmarker.Tag = green;                        
                        AddGreenMarker(currentmarker);
                    }
                foreach (var blue in route.NavigationMarkers) if (blue.Lat == item.Lat && blue.Lng == item.Lng)
                    {
                        currentmarker.Tag = blue;
                        AddBlueMarker(currentmarker);
                    }
            }
            UpdateRoute();
            Map.ZoomAndCenterRoute(RedRoute);
        }
        /// <summary>
        /// Copy overlays into Route class.
        /// </summary>
        /// <param name="route"></param>
        public void CopyTo(Route route)
        {
            route.GpsMarkers.Clear();
            route.ChangeMarkers.Clear();
            route.NavigationMarkers.Clear();
            foreach (var item in Red.Markers)
            {
                route.GpsMarkers.Add(new GpsMarker(item.Position.Lng, item.Position.Lat));
                if (item.Tag is ChangeMarker)
                    route.ChangeMarkers.Add(item.Tag as ChangeMarker);
                if (item.Tag is NavigationMarker) route.NavigationMarkers.Add(item.Tag as NavigationMarker);
            }
        }

        public void EditMarker(MouseEventArgs e)
        {
            var originalTag = CurrentMarker.Tag;
            var before = (CurrentMarker.Tag != null) ? 2 : 0;
            Form form = null;
            if (CurrentMarker.Tag is ChangeMarker)
                form = new FormEditChangeMarker(CurrentMarker);
            else if (CurrentMarker.Tag is NavigationMarker)
                form = new FormEditChangeMarker(CurrentMarker);
            else if (CurrentMarker.Tag == null) // Empty GpsMarkers contains integer id.
                form = new FormEditChangeMarker(CurrentMarker); //TODO key to Edit NavigationMarker
            else My.Log($"Error during edit Tag {CurrentMarker.Tag}");

            form.ShowDialog();
            //
            // Determine CRUD operation (Create, Update or Delete).
            // 0= No operation, 1 = Create, 2 = Delete, 3 = Update.
            //
            var after = (CurrentMarker.Tag != null) ? 1 : 0;
            var crud = (Crud)before + after;

            UpdateGreenAndBlueOverlay(crud, originalTag, CurrentMarker.Tag);
        }

        private void UpdateGreenAndBlueOverlay(Crud crud, object origin, object tag)
        {
            if (tag!=null) Console.WriteLine($"{crud} {tag.ToString().Replace('\n', ' ')}");
            else if (origin!=null) Console.WriteLine($"{crud} {origin.ToString().Replace('\n', ' ')}");

            switch (crud)
            {
                case Crud.None:
                    break;
                case Crud.Create:
                    if (tag is ChangeMarker) AddGreenMarker(CurrentMarker);
                    if (tag is NavigationMarker) AddBlueMarker(CurrentMarker);
                    break;
                case Crud.Delete:
                    if (origin is ChangeMarker) foreach (var item in Green.Markers) if (item.Tag == origin)
                            {
                                Green.Markers.Remove(item); return;
                            }
                    if (origin is NavigationMarker) foreach (var item in Blue.Markers) if (item.Tag == origin)
                            {
                                Blue.Markers.Remove(item); return;
                            }
                    break;
                case Crud.Update:
                    if (tag is ChangeMarker) Green.Markers[Green.Markers.Count - 1].ToolTipText = CurrentMarker.Tag.ToString();
                    if (tag is NavigationMarker) Blue.Markers[Blue.Markers.Count - 1].ToolTipText = CurrentMarker.Tag.ToString();

                    break;
                default:
                    My.Log($"Error crud operation {crud}");
                    break;
            }
        }

        private void AddBlueMarker(GMapMarker currentMarker)
        {
            Blue.Markers.Add(new GMarkerGoogle(currentMarker.Position, GMarkerGoogleType.blue_small));
            Blue.Markers[Blue.Markers.Count - 1].Tag = currentMarker.Tag;
            Blue.Markers[Blue.Markers.Count - 1].ToolTipText = currentMarker.Tag.ToString();
        }

        private void AddGreenMarker(GMapMarker currentMarker)
        {
            Green.Markers.Add(new GMarkerGoogle(currentMarker.Position, GMarkerGoogleType.green_small));
            Green.Markers[Green.Markers.Count - 1].Tag = currentMarker.Tag;
            Green.Markers[Green.Markers.Count - 1].ToolTipText = currentMarker.Tag.ToString();
        }

        private enum Crud { None, Create, Delete, Update }
    }
}
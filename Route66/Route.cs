using GMap.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Route66
{
    public enum MachineTypes
    {
        StandardSpreader,
        WspPercentage,
        WspDosage,
        RspPercentage,
        RspDosage,
        Sprayer,
        Dst,
        RspDstPercentage,
        WspDstPercentage,
        StreetWasher
    }
    public class Route
    {
        #region FIELDS
        private static Route route;
        private string fileName;
        public static bool IsDefaultFile;
        #endregion
        #region CONSTRUCTOR
        public Route()
        {
            MachineType = MachineTypes.StandardSpreader;
            Version = "1.0";
            GpsMarkers = new List<GpsMarker>();
            ChangeMarkers = new List<ChangeMarker>();
            NavigationMarkers = new List<NavigationMarker>();
        }
        #endregion
        #region METHODES
        public static Route Load(string fileName = "Route66.xml")
        {
            IsDefaultFile = (fileName == "Route66.xml");
            route = new Route();
            try
            {
                using (TextReader reader = new StreamReader(fileName))
                {
                    route = new XmlSerializer(typeof(Route)).Deserialize(reader) as Route;
                }
            }
            catch (Exception exception)
            {
                if (!IsDefaultFile && MessageBox.Show(exception.Message + "\nWould you like to load default settings?", "Loading application settings.", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    Environment.Exit(1);
                }
            }
            route.fileName = fileName;
            return route;
        }
        public void Save() => SaveAs(fileName);
        public void SaveAs(string fileName)
        {
            try
            {
                var dir = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                XmlSerializer serializer = new XmlSerializer(typeof(Route));
                using (StreamWriter writer = new StreamWriter(fileName)) serializer.Serialize(writer, this);
                route.fileName = fileName;
                IsDefaultFile = (fileName == "Route66.xml");
                My.Status($"Route saved to {fileName}");
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
        #endregion
        #region PROPERTIES
        public MachineTypes MachineType { get; set; }
        public String Version { get; set; }
        public List<GpsMarker> GpsMarkers { get; set; }
        public List<ChangeMarker> ChangeMarkers { get; set; }
        public List<NavigationMarker> NavigationMarkers { get; set; }

        #endregion
    }

    public class NavigationMarker : GpsMarker
    {
        public NavigationMarker() { }
        public NavigationMarker(PointLatLng position) : base(position.Lng, position.Lat)
        {
            SoundFile = "EnterSoundfile.wav";
            Msg = "Turn right";
        }
        public override string ToString()
        {
            return $"{Msg} {SoundFile}";
        }
        #region PROPERTIES
        [XmlAttribute()]
        public string SoundFile { get; set; }
        [XmlAttribute()]
        public string Msg { get; set; }
        #endregion
    }

    public class ChangeMarker : GpsMarker
    {
        public ChangeMarker() { }
        public ChangeMarker(PointLatLng position) : base(position.Lng, position.Lat)
        {
            Dosing = 20.0;
            WidthLeft = 1.0;
            WidthRight = 1.0;
        }
        public override string ToString()
        {
            return $"Dosing {Dosing}\nWidthLeft {WidthLeft}\nWidthRight {WidthRight}";
        }
        [XmlAttribute()]
        public double Dosing { get; set; }
        [XmlAttribute()]
        public double WidthLeft { get; set; }
        [XmlAttribute()]
        public double WidthRight { get; set; }
    }

    public class GpsMarker
    {
        public GpsMarker(double lng, double lat)
        {
            Lng = lng;
            Lat = lat;
        }
        public GpsMarker()
        {
            Lng = 4.0;
            Lat = 52.0;
        }
        [XmlAttribute()]
        public double Lng { get; set; }
        [XmlAttribute()]
        public double Lat { get; set; }
    }
}

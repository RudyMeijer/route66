using GMap.NET;
using MyLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Route66
{
	public class Route
	{
		#region FIELDS
		//[XmlIgnore]
		internal string FileName;
		public static bool IsDefaultFile;
		internal bool IsNotSupported;
		//[XmlIgnore]
		internal bool IsChanged;
		private static Route route;
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
			route.FileName = fileName;
			route.IsChanged = false;
			try
			{
				using (TextReader reader = new StreamReader(fileName))
				{
					switch (Path.GetExtension(fileName))
					{
						case ".xml":
							route = new XmlSerializer(typeof(Route)).Deserialize(reader) as Route;
							break;
						case ".ar3":
							ReadAr3(reader, route);
							break;
						default: route.IsNotSupported = true; break;
					}
				}
			}
			catch (Exception ee)
			{
				if (!IsDefaultFile)
				{
					if (ee.InnerException != null)
						My.Show($"{ee.InnerException.Message}", ee.Message);
					else
						My.Show($"{ee.Message}", "Loading route.");
				}
			}
			return route;
		}

		private static void ReadAr3(TextReader reader, Route route)
		{
			var line = "";
			var version = "";
			var provider = CultureInfo.GetCultureInfo("en").NumberFormat;
			var distanceTable = new Dictionary<String, PointLatLng>();
			var err1 = 0;
			var err2 = 0;
			while ((line = reader.ReadLine()) != null)
			{
				try
				{
					var s = line.Split(':', ',');

					if (line.StartsWith("Ar3")) version = line.Split(':')[1];
					else if (line.StartsWith("MachineType")) route.MachineType = My.GetEnum<MachineTypes>(s[1]);
					else if (line.StartsWith("WayPoint["))
					{
						var point = new PointLatLng(Double.Parse(s[2], provider), Double.Parse(s[1], provider));
						route.GpsMarkers.Add(new GpsMarker(point));
						distanceTable.Add(s[3], point);
					}
					else if (line.StartsWith("Instruction[")) route.NavigationMarkers.Add(new NavigationMarker(FindLatLng(s[1])));
					else if (line.StartsWith("ChangePoint[")) route.ChangeMarkers.Add(new ChangeMarker(FindLatLng(s[1])));
				}
				catch (KeyNotFoundException)
				{
					++err1;
					My.Log($"{line} not found in waypoints."); 
				}
				catch (Exception)
				{
					++err2;
					My.Log($"Duplicated line {line}"); 
				}
			}
			if (err1+err2 > 0) My.Show($"Total {err1 + err2} errors in route {Path.GetFileName(route.FileName)} detected. \n{err2} duplicated lines will be ignored. {err1} missing waypoints will be added.\nSee logfile for more information.", $"Statistical report.");

			PointLatLng FindLatLng(string distance) => distanceTable[distance];
		}


		public void Save() => SaveAs(FileName);
		public void SaveAs(string fileName)
		{
			try
			{
				var dir = Path.GetDirectoryName(fileName);
				if (!Directory.Exists(dir) && dir.Length > 0) Directory.CreateDirectory(dir);

				XmlSerializer serializer = new XmlSerializer(typeof(Route));
				using (StreamWriter writer = new StreamWriter(fileName)) serializer.Serialize(writer, this);
				route.FileName = fileName;
				IsDefaultFile = (fileName == "Route66.xml");
				My.Status($"Saved route succesfull to {this}");
				IsChanged = false;
			}
			catch (Exception e)
			{
				throw new Exception(e.ToString());
			}
		}
		public override string ToString()
		{
			return $"{FileName} V{Version}, Type {MachineType}";
		}
		#endregion
		#region PROPERTIES
		public MachineTypes MachineType { get; set; }
		public String Version { get; set; }
		public List<GpsMarker> GpsMarkers { get; set; }
		public List<ChangeMarker> ChangeMarkers { get; set; }
		public List<NavigationMarker> NavigationMarkers { get; set; }

		internal void Clear()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

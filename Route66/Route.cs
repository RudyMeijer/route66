using GMap.NET;
using MyLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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
			route.FileName = fileName;
			route.IsChanged = false;
			return route;
		}

		private static void ReadAr3(TextReader reader, Route route)
		{
			var line = "";
			var version = "";
			var provider = CultureInfo.GetCultureInfo("en").NumberFormat;
			var distanceTable = new Dictionary<int, PointLatLng>();
			int err1 = 0, err2 = 0, err3 = 0;
			var sb = new StringBuilder();
			var lastKey = 0;
			var lastDistance = -1;
			while ((line = reader.ReadLine()) != null)
			{
				try
				{
					var s = line.Split(':', ',');

					if (line.StartsWith("Ar3")) version = s[1];
					else if (line.StartsWith("MachineType")) route.MachineType = My.GetEnum<MachineTypes>(s[1]);
					else if (line.StartsWith("WayPoint["))
					{
						var point = new PointLatLng(Double.Parse(s[2], provider), Double.Parse(s[1], provider));
						route.GpsMarkers.Add(new GpsMarker(point));
						var distance = int.Parse(s[3]);
						if (distance < lastDistance) { ++err1; sb.Append($"\n{line} has descending distance and will be ignored."); }
						else if (distance == lastDistance) { ++err2; sb.Append($"\nDuplicated line {line}"); }
						else distanceTable.Add(distance, point);
						lastDistance = distance;
					}
					else if (line.StartsWith("Instruction[")) route.NavigationMarkers.Add(new NavigationMarker(FindLatLng(s[1])));
					else if (line.StartsWith("ChangePoint[")) route.ChangeMarkers.Add(new ChangeMarker(FindLatLng(s[1])));
				}
				catch (Exception ee) { ++err3; sb.Append($"\nError in {line} {ee.Message}"); }
			}
			if (err1 + err2 + err3 > 0)
			{
				My.Log(sb.ToString());
				My.Show($"Total {err1 + err2 + err3} errors in route {Path.GetFileName(route.FileName)} detected. \n{err2} duplicated lines will be ignored.\n{err1} points have descending distance and will be ignored. \n{err3} other errors. \nMissing waypoints will be added.\nAll errors are succesfully resolved. See logfile for more information.", $"Statistical report.");
			}

			PointLatLng FindLatLng(string distance)
			{
				int d = int.Parse(distance);
				lastKey = 0;
				foreach (var item in distanceTable)
				{
					if (item.Key >= d)
					{
						distanceTable.Remove(item.Key);// Use distance only one's so that not both NP and CP can be added to one gps point.
						return item.Value;
					}
					else if (item.Key < lastKey) sb.Append($"\nDistance {line} not accending.");

					lastKey = item.Key;
				}
				return distanceTable[0]; //todo
			}
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

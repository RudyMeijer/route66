﻿using GMap.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Route66
{
	public class Route
	{
		#region FIELDS
		[XmlIgnore]
		public string FileName;
		public static bool IsDefaultFile;
		[XmlIgnore]
		public bool IsChanged;
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
			My.Log($"Open route {fileName }");
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
				if (!IsDefaultFile && MessageBox.Show(exception.Message + "\nWould you like to load default route?", "Loading route.", MessageBoxButtons.YesNo) == DialogResult.No)
				{
					Environment.Exit(1);
				}
			}
			route.FileName = fileName;
			route.IsChanged = false;
			return route;
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
				My.Status($"Route saved to {fileName}");
				IsChanged = false;
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
}

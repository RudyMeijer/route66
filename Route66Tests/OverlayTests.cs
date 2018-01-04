using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLib;
using Route66;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Route66.Tests
{
	[TestClass()]
	public class OverlayTests
	{
		private readonly ToolStripStatusLabel status;
		private GMapControl gmap = new GMapControl();
		public OverlayTests()
		{
			status = new System.Windows.Forms.ToolStripStatusLabel();
			gmap.AutoScaleDimensions = new System.Drawing.SizeF(100, 200);
			gmap.MapProvider = BingMapProvider.Instance;
			GMaps.Instance.Mode = AccessMode.ServerAndCache;
			gmap.Zoom = 13;
			My.SetStatus(status);
			Settings = Settings.Load();
		}

		public Settings Settings { get; }

		[TestMethod(), DeploymentItem(@"..\..\Test data\")]
		public void OpenRouteTest()
		{
			var filename = "angle.xml";
			var d = My.CurrentDirectory;
			var overlay = new Overlay(gmap);
			Settings.MachineType = MachineTypes.Dst;
			var r = overlay.OpenRoute(filename);
			Assert.IsTrue(r == true, "Couldn't Open Route");
			//
			// Test conversion.
			//
			Assert.IsTrue(status.Text.StartsWith("Route succ"), "Couldn't convert Route {}");
			//
			// Update dosage from 33 to 30 gr/m2
			//
			Assert.IsTrue((int)My.Invoke(overlay, "UpdateDosageAllChangeMarkers", 33, 30) == 1, "Error updating dosage.");
			//
			// Get red marker 1 and check for IsGpsMarker.
			//
			var marker = gmap.Overlays[0].Markers[1];
			Assert.IsTrue(overlay.IsGpsMarker(marker), "Error IsGpsMarker.");
			//
			// Check green marker tag dosage.
			//
			var cm = My.Invoke(overlay, "GetGreenMarker", marker) as GMap.NET.WindowsForms.Markers.GMarkerGoogle;
			Assert.IsTrue((cm.Tag as ChangeMarker).Dosage == 30, "Error in ChangeMarker");
			//
			// Get angle red marker 1.
			//
			var angle = (float)My.Invoke(overlay, "Angle", marker);
			Assert.IsTrue(angle == 0f, "Error in Angle");
			//
			// Test filename.
			//
			var file = (string)My.GetField(overlay.Route, "FileName");
			Assert.IsTrue(filename == file, "Error in route filename.");
		}
	}
}
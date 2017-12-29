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
			gmap.SetBounds(0, 0, 100, 100);
			My.SetStatus(status);
			Settings = Settings.Load();
		}

		public Settings Settings { get; }

		[TestMethod(), DeploymentItem(@"..\..\Test data\")]
		public void OpenRouteTest()
		{
			var d = My.CurrentDirectory;
			var overlay = new Overlay(gmap);
			Settings.MachineType = MachineTypes.Dst;
			var r = overlay.OpenRoute("angle.xml");
			Assert.IsTrue(r == true, "Couldn't Open Route");
			//
			// Test conversion.
			//
			Assert.IsTrue(status.Text.StartsWith("Route succ"), "Couldn't convert Route {}");
			//
			// Update dosage from 20 to 30 gr/m2
			//
			Assert.IsTrue((int)My.Invoke(overlay, "UpdateDosageAllChangeMarkers", 20, 30) == 1, "Error updating dosage.");
			//
			// Get red marker 1 and check for IsGpsMarker.
			//
			var marker = gmap.Overlays[0].Markers[1];
			Assert.IsTrue(overlay.IsGpsMarker(marker), "Error updating dosage.");
			//
			// Get angle red marker 1.
			//
			var angle = My.Invoke(overlay, "Angle", marker);
		}
	}
}
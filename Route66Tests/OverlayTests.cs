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
			My.SetStatus(status);
			Settings = Settings.Load();
		}

		public Settings Settings { get; }

		[TestMethod(), DeploymentItem(@"..\..\..\Route66\Test data\angle.xml")]
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
			// Get red marker 1.
			//
			var marker = gmap.Overlays[0].Markers[1];
			//
			// Get angle red marker 1.
			//
			var bf = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod;
			
			var m = overlay.GetType().GetMethod("Angle",bf);
			var angle = m.Invoke(overlay, new object[] { marker });
		}
	}
}
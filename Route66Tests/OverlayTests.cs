﻿using GMap.NET.WindowsForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLib;
using Route66;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Route66.Tests
{
	[TestClass()]
	public class OverlayTests
	{
		private GMapControl gmap= new GMapControl();
		public OverlayTests()
		{
			My.SetStatus(new System.Windows.Forms.ToolStripStatusLabel());
		}
		[TestMethod()]
		public void OpenRouteTest()
		{
			var overlay = new Overlay(gmap);
			var r = overlay.OpenRoute("angle.xml");
			Assert.IsTrue(r==true,"Couldn't Open Route {}");
		}
	}
}
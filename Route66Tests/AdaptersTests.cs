using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLib;
using Route66;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Route66.Tests
{
	[TestClass()]
	public class AdaptersTests
	{
		[TestMethod(), DeploymentItem(@"..\..\Test data\")]
		public void ReadAr3Test()
		{
			var filename = "KleineRoute.ar3";
			var route = Adapters.ReadAr3(filename);
			Assert.IsFalse(Route.IsDefaultFile, "ReadAr3 error 0");
			Assert.IsTrue(route.GpsMarkers.Count == 594, "ReadAr3 error 1");
			Assert.IsTrue(route.ChangeMarkers.Count == 66, "ReadAr3 error 2");
			Assert.IsTrue(route.NavigationMarkers.Count == 56, "ReadAr3 error 3");
			//
			// Test last navigation marker.
			//
			var np = route.NavigationMarkers[55];
			Assert.IsTrue(np.Lat == 52.2769583333333, "ReadAr3 error 4");
			Assert.IsTrue(np.SoundFile == "Arrive.wav", "ReadAr3 error 5");
			//
			// Test internal Filename.
			//
			Assert.IsTrue((string)My.GetField(route, "FileName") == filename, "ReadAr3 error 6");
			//
			// Test requirements.
			//
			Assert.IsTrue(Adapters.errors[0] == 1, "Requirement violation 7");
			Assert.IsTrue(Adapters.errors[1] == 4, "Requirement violation 8");
			Assert.IsTrue(Adapters.errors[2] == 0, "Requirement violation 9");
			Assert.IsTrue(Adapters.errors[3] == 0, "Requirement violation 10");
			Assert.IsTrue(Adapters.errors[4] == 112, "Requirement violation 11");
		}
	}
}
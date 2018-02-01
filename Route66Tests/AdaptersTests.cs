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
		[TestMethod, DeploymentItem(@"..\..\Test data\")]
		public void ReadAr3Test()
		{
			var filename = "KleineRoute.ar3";
			var route = Adapters.ReadAr3(filename);
			Assert.IsFalse(Route.IsDefaultFile, "ReadAr3 error 0");
			Assert.IsTrue(route.GpsMarkers.Count == 595, "ReadAr3 error 1");
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
        [TestMethod, DeploymentItem(@"..\..\Test data\")]
        public void RequirementsAr3Test()
        {
            var p = My.CurrentDirectory;
            var filename = "Requirements test.ar3";
            var route = Adapters.ReadAr3(filename);
            Assert.IsTrue(route.GpsMarkers.Count == 5, "Requirement violation 12");
            Assert.IsTrue(route.NavigationMarkers.Count == 1, "Requirement violation 13");
            Assert.IsTrue(route.ChangeMarkers.Count == 3, "Requirement violation 14");
            //
            // Check errors.
            //
            var e= Adapters.errors;
            Assert.IsTrue(e[0]==1, "Points have descending distance and will be ignored.");
            Assert.IsTrue(e[1]==1, "Duplicated line.");
            Assert.IsTrue(e[2]==1, "Unkown navigation type.");
            Assert.IsTrue(e[3]==2, "Exception.");
            Assert.IsTrue(e[4]==1, "Orphan.");
            //
            // Write ar3 and read back.
            //
            filename = "Requirements test wr.ar3";
            Adapters.WriteAr3(filename, route);
            var route2 = Adapters.ReadAr3(filename);
            Assert.IsTrue(route2.GpsMarkers.Count == 5, "Requirement violation 22");
            Assert.IsTrue(route2.NavigationMarkers.Count == 1, "Requirement violation 23");
            Assert.IsTrue(route2.ChangeMarkers.Count == 3, "Requirement violation 24");
        }
    }
}
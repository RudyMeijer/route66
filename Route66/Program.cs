using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Route66
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			TestReflection();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try
			{
				Application.Run(new Form1());
			}
			catch (Exception e)
			{
				var msg = $"{e} {e.InnerException}";
				My.Log(msg);
				MessageBox.Show(msg, "Sometimes you have luck sometimes not. Please send file Route.log to Rudy.");
			}
		}

		private static void TestReflection()
		{
			var obj = new Route();
			var type = typeof(object);
			var properties = obj.GetType().GetProperties();
			var methodes = obj.GetType().GetMethods();
		}
	}
}

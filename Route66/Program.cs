using MyLib;
using System;
using System.Collections;
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
			TestStack();
			//TestReflection();
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

		private static void TestStack()
		{
			var stack = new Stack();
			stack.Push(1);
			stack.Push(2);
			stack.Push(3);

			stack.Pop();
			var x = stack.Peek();
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

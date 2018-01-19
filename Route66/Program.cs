// <copyright file="Program.cs" company="Aebi Schmidt Nederland B.V.">
//   Aebi Schmidt Nederland B.V. All rights reserved.
// </copyright>

namespace Route66
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using MyLib;

    /// <summary>
    /// This class contains the main entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            //// TestFloat();
            //// TestStack();
            //// TestReflection();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new Form1());
            }
            catch (Exception e)
            {
                My.Show($"{e} {e.InnerException}", "Sometimes you have luck sometimes not. Please send file Route66.log to Rudy.");
            }
        }

        ////private static void TestFloat()
        ////{
        ////    float R = 0.82f;
        ////    var L = R * 1000;
        ////    int i = (int)(R * 1000);
        ////    int j = (int)(L);
        ////}

        ////private static void TestStack()
        ////{
        ////    var stack = new Stack();
        ////    stack.Push(1);
        ////    stack.Push(2);
        ////    stack.Push(3);

        ////    stack.Pop();
        ////    var x = stack.Peek();
        ////}

        ////private static void TestReflection()
        ////{
        ////    var obj = new Route();
        ////    var type = typeof(object);
        ////    var properties = obj.GetType().GetProperties();
        ////    var methodes = obj.GetType().GetMethods();
        ////}
    }
}

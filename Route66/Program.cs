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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool retry=false;
            do try
            {
                Application.Run(new Form1());
            }
            catch (Exception e)
            {
                var msg = $"{e} {e.InnerException}";
                My.Log(msg);
                retry = MessageBox.Show(msg, "Sometimes you have luck sometimes not. Please send file Route.log to Rudy.", MessageBoxButtons.RetryCancel) == DialogResult.Retry;
            }
            while (retry);
        }
    }
}

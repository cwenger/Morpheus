using System;
using System.Windows.Forms;

namespace Morpheus
{
    static class Program
    {
        internal static string GetProductNameAndVersion()
        {
            string name = Application.ProductName;
            int revision = new Version(Application.ProductVersion).Revision;
            string name_and_version = name.Insert(name.IndexOf(')'), ", revision " + revision.ToString());
            return name_and_version;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
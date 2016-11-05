using System;
using System.Threading;
using System.Windows.Forms;

namespace R2D2Remote
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
            Form1 f = new Form1();

            ControlInterface controls = new KeyboardControl();
            controls.SetThrottle = f.SetThrottle;
            controls.Init();
            Application.Run(f);
 

        }
    }
}

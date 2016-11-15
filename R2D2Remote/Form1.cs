using System;
using System.Windows.Forms;
using System.Threading;

namespace R2D2Remote
{
    public partial class Form1 : Form
    {
        private R2D2Networking robotCom;
        private float throttle = 0;
        public Form1()
        {

            InitializeComponent();
            robotCom = new R2D2Networking("R2D2.local");
            robotCom.Start();
            new Thread(() =>
            {
                for (;;)
                {
                    robotCom.SendValue(R2D2Networking.ValueType.throttle, throttle);
                    Thread.Sleep(10);
                }

            }).Start();
        }

        delegate void SetTextCallback(float value);
        public void SetThrottle(float f)
        {
            throttle = f;
            if (this.trackBar1.InvokeRequired)
            {
                try
                {
                    SetTextCallback d = new SetTextCallback(SetThrottle);
                    this.Invoke(d, new object[] { f });
                }
                catch (Exception) { }

            }
            else
            {
                trackBar1.Value = (int)(f * 100);
            }
        }
    }
}

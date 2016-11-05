using System;
using System.Windows.Forms;

namespace R2D2Remote
{
    public partial class Form1 : Form
    {
        private R2D2Networking robotCom;
        public Form1()
        {

            InitializeComponent();
            robotCom = new R2D2Networking("R2D2.local");
            robotCom.Start();
        }

        delegate void SetTextCallback(float value);
        public void SetThrottle(float f)
        {
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
                robotCom.SendValue(R2D2Networking.ValueType.throttle,f);
                trackBar1.Value = (int)(f * 100);
            }
        }
    }
}

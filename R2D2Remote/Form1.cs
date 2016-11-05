using System;
using System.Windows.Forms;

namespace R2D2Remote
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
                trackBar1.Value = (int)(f * 100);
            }
        }
    }
}

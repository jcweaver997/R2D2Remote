using System;
using System.Windows.Forms;
using System.Threading;

namespace R2D2Remote
{
    public partial class Form1 : Form
    {
        private R2D2Networking robotCom;
        private float throttle = 0;
        private float turn = 0;
        delegate void ShowValueDelegate(TrackBar t, float value);
        private ShowValueDelegate showValue;
        public Form1()
        {


            InitializeComponent();
            new Thread(() =>
            {
                robotCom = new R2D2Networking("R2D2.local");
                robotCom.Start();
                while (Visible)
                {
                    robotCom.SendValue(R2D2Networking.ValueType.throttle, throttle);
                    showValue = new ShowValueDelegate(ShowValue);
                    this.Invoke(showValue, new object[] {trackBar1, throttle });
                    robotCom.SendValue(R2D2Networking.ValueType.turn, turn);
                    showValue = new ShowValueDelegate(ShowValue);
                    this.Invoke(showValue, new object[] {trackBar2,  turn });
                    Thread.Sleep(50);
                }

            }).Start();
        }


        public void ShowValue(TrackBar t, float value)
        {
            t.Value = (int)(value*100);
        }
        public void SetThrottle(float f)
        {
            throttle = f;
        }
        public void SetTurn(float f)
        {
            turn = f;
        }
    }
}

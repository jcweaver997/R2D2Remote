using System;
using System.Windows.Forms;
using System.Threading;

namespace R2D2Remote
{
    public partial class Form1 : Form
    {
        public static Form1 instance;
        private R2D2Networking robotCom;
        private float[] values;
        delegate void ShowValueDelegate(TrackBar t, float value);
        private ShowValueDelegate showValue;
        private ControlInterface controls;
        public Form1(ControlInterface control)
        {
            values = new float[3];
            this.controls = control;
            instance = this;
            showValue = ShowValue;
            controls.SetValue = SetValue;
            controls.Init(this);
            InitializeComponent();
            new Thread(() =>
            {
                robotCom = new R2D2Networking("R2D2.local");
                robotCom.Start();
                int count = 0;
                while (Visible)
                {
                    count++;
                    Thread.Sleep(50);
                    try // Needed for when the window closes
                    {
                        controls.ReadInput();
                        robotCom.SendValue(R2D2Networking.ValueType.throttle, values[(int)R2D2Networking.ValueType.throttle]);

                        this.Invoke(showValue, new object[] { trackBar1, values[(int)R2D2Networking.ValueType.throttle] });
                        robotCom.SendValue(R2D2Networking.ValueType.turn, values[(int)R2D2Networking.ValueType.turn]);
                        this.Invoke(showValue, new object[] { trackBar2, values[(int)R2D2Networking.ValueType.turn] });
                        Console.WriteLine(count);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message+"\n"+e.StackTrace);
                    }

                }

            }).Start();
        }


        public void ShowValue(TrackBar t, float value)
        {
            t.Value = (int)(value*100);
        }

        public void SetValue(int id, float value)
        {
            values[id] = value;
        }

    }
}

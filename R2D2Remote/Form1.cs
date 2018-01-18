using System;
using System.Windows.Forms;
using System.Threading;
using R2D2PI;

namespace R2D2Remote
{
    public partial class Form1 : Form
    {
        public static Form1 instance;
        private R2D2Connection robotCom;
        private float[] values;
        delegate void ShowValueDelegate(TrackBar t, float value);
        private ShowValueDelegate showValue;
        private KeyboardControl controls;
        public Form1()
        {
            values = new float[3];
            instance = this;
            showValue = ShowValue;
            controls = new KeyboardControl();
            controls.Init(this,SetValue);
            InitializeComponent();
            new Thread(() =>
            {
            robotCom = new R2D2Connection(R2D2Connection.ConnectionType.Controller);
                robotCom.Connect();
                int count = 0;
                while (Visible)
                {
                    count++;
                    Thread.Sleep(50);
                    try // Needed for when the window closes
                    {

                        robotCom.SendCommand(new R2D2Connection.Command(R2D2Connection.Commands.SetLeftDriveMotor, BitConverter.GetBytes(values[0])));
                        robotCom.SendCommand(new R2D2Connection.Command(R2D2Connection.Commands.SetRightDriveMotor, BitConverter.GetBytes(values[1])));

                        this.Invoke(showValue, new object[] { trackBar1, values[0] });
                        this.Invoke(showValue, new object[] { trackBar2, values[1] });

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

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
                        float[] gtaval = TranslateValuesToGTA(values[0], values[1]);
                        robotCom.SendCommand(new R2D2Connection.Command(R2D2Connection.Commands.SetLeftDriveMotor, BitConverter.GetBytes(gtaval[0])));
                        robotCom.SendCommand(new R2D2Connection.Command(R2D2Connection.Commands.SetRightDriveMotor, BitConverter.GetBytes(gtaval[1])));

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

        private float[] TranslateValuesToGTA(float throttle, float turn)
        {
            float[] ar = new float[2];
            if (throttle>.5f)
            {
                ar[0] = throttle;
                ar[1] = throttle;
                if (turn>0)
                {
                    ar[1] -= turn * 1.5f;
                }
                else
                {
                    ar[0] -= turn * 1.5f;
                }

            }
            else if (throttle < -.5f)
            {
                ar[0] = throttle;
                ar[1] = throttle;
                if (turn > 0)
                {
                    ar[1] += turn * 1.5f;
                }
                else
                {
                    ar[0] += turn * 1.5f;
                }

            }
            else
            {
                ar[0] = throttle;
                ar[1] = throttle;
                ar[0] += turn * 1f;
                ar[1] -= turn * 1f;
            }
            return ar;
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

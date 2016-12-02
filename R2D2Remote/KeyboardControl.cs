
using Gma.System.MouseKeyHook;
using System;
using System.Windows.Forms;

namespace R2D2Remote
{
    class KeyboardControl : ControlInterface
    {
        float[] controls = new float[2];
        IKeyboardMouseEvents HookManager;
        public override void Init()
        {
            HookManager = Hook.GlobalEvents();
            Console.WriteLine("starting up");
            HookManager.KeyDown += new KeyEventHandler(KeyDown);
            HookManager.KeyUp += new KeyEventHandler(KeyUp);
        }

        public override float GetThrottle()
        {
            return controls[0];
        }

        public override float GetTurn()
        {
            return controls[1];
        }

        private void KeyDown(Object s, KeyEventArgs a)
        {
            switch (a.KeyCode)
            {
                case Keys.W:
                    controls[0] = 1;
                    break;
                case Keys.S:
                    controls[0] = -1;
                    break;
                case Keys.A:
                    controls[1] = -1;
                    break;
                case Keys.D:
                    controls[1] = 1;
                    break;
            }
            SetThrottle(GetThrottle());
            SetTurn(GetTurn());
        }
        private void KeyUp(Object s, KeyEventArgs a)
        {
            switch (a.KeyCode)
            {
                case Keys.W:
                    controls[0] = 0;
                    break;
                case Keys.S:
                    controls[0] = 0;
                    break;
                case Keys.A:
                    controls[1] = 0;
                    break;
                case Keys.D:
                    controls[1] = 0;
                    break;
            }
            SetThrottle(GetThrottle());
            SetTurn(GetTurn());
        }
    }
}

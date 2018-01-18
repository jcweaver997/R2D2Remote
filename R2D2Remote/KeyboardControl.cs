
using Gma.System.MouseKeyHook;
using System;
using System.Windows.Forms;

namespace R2D2Remote
{
    class KeyboardControl
    {
        private float[] controls = new float[2];
        private IKeyboardMouseEvents HookManager;
        private Form form;
        public delegate void SetValue(int id, float val);
        private SetValue setValue;
        public void Init(Form form, SetValue setValue)
        {
            this.form = form;
            this.setValue = setValue;
            HookManager = Hook.GlobalEvents();
            Console.WriteLine("starting up");
            HookManager.KeyDown += new KeyEventHandler(KeyDown);
            HookManager.KeyUp += new KeyEventHandler(KeyUp);
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

            //setValue(0, controls[0]);
            //setValue(1, controls[1]);
            form.Invoke(setValue, new Object[] { 0, controls[0] });
            form.Invoke(setValue, new Object[] { 1, controls[1] });
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
            form.Invoke(setValue, new Object[] { 0, controls[0] });
            form.Invoke(setValue, new Object[] { 1, controls[1] });
        }
    }
}

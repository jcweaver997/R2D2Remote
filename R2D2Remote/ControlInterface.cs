using System.Windows.Forms;

namespace R2D2Remote
{
    public abstract class ControlInterface
    {
        public delegate void SetValueDelegate(int id, float value);
        public SetValueDelegate SetValue;
        public abstract void Init(Form f);
        public abstract float GetThrottle();
        public abstract float GetTurn();
        public abstract void ReadInput();
    }
}

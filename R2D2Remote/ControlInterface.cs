namespace R2D2Remote
{
    abstract class ControlInterface
    {
        public delegate void SetValue(float value);
        public SetValue SetThrottle;
        public abstract void Init();
        public abstract float GetThrottle();
        public abstract float GetTurn();
    }
}

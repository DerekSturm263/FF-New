namespace Quantum
{
    [System.Serializable]
    public abstract unsafe partial class Ultimate : InfoAsset
    {
        public MoveRef Move;
        public int Length;

        public abstract void OnBegin(Frame f, EntityRef user);
        public abstract void OnEnd(Frame f, EntityRef user);
    }
}

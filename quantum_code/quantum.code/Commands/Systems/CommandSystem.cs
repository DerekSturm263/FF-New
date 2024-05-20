namespace Quantum
{
    public unsafe class CommandSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            for (int i = 0; i < f.PlayerCount; i++)
            {
                var command = f.GetPlayerCommand(i) as CommandSetBuild;
                command?.Execute(f);
            }
        }
    }
}

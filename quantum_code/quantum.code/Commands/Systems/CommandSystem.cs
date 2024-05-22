namespace Quantum
{
    public unsafe class CommandSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            for (int i = 0; i < f.PlayerCount; i++)
            {
                var command = f.GetPlayerCommand(i);

                if (command is CommandSetBuild commandSetBuild)
                    commandSetBuild?.Execute(f);
                else if (command is CommandSetStage commandSetStage)
                    commandSetStage?.Execute(f);
                else if (command is CommandSetRuleset commandSetRuleset)
                    commandSetRuleset?.Execute(f);
            }
        }
    }
}

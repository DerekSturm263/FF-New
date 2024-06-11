﻿using Photon.Deterministic;

namespace Quantum
{
    public unsafe class CommandPlayerApplyProfile : DeterministicCommand
    {
        public EntityRef entity;
        public QString32 name;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref entity);
            stream.Serialize(ref name);
        }

        public void Execute(Frame f)
        {
            Log.Debug("Profile applied!");

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
                stats->Name = name;
        }
    }
}
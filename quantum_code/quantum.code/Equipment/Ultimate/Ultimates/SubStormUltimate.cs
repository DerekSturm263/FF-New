﻿using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class SubStormUltimate : Ultimate
    {
        public override void OnBegin(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                stats->EnergyModifyMultiplier = 0;
            }
        }

        public override void OnEnd(Frame f, EntityRef user)
        {
            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                stats->EnergyModifyMultiplier = 1;
            }
        }
    }
}
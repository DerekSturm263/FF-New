using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public abstract unsafe partial class Ultimate
    {
        public AnimationRef Move;
        public int Length;

        public abstract void OnBegin(Frame f, EntityRef user);
        public abstract void OnEnd(Frame f, EntityRef user);
    }
}

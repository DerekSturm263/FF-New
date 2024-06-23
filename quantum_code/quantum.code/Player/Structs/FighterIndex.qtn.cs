using Photon.Deterministic;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Quantum
{
    public unsafe partial struct FighterIndex
    {
        public static FighterIndex Invalid = new()
        {
            Local = -1,
            Global = -1,
            Internal = -1
        };

        public override string ToString() => $"(Local: {Local}, Global: {Global})";
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    [System.Serializable]
    public partial class MatchAsset : InfoAsset
    {
        public Match Match;
        public Bot[] Opponents;
    }
}

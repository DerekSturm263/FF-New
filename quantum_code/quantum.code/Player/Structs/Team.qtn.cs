﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum
{
    public unsafe partial struct Team
    {
        public IEnumerable<FighterIndex> Get(Frame f)
        {
            List<FighterIndex> list = [];

            if (!Item1.Equals(default(FighterIndex)))
                list.Add(Item1);
            if (!Item2.Equals(default(FighterIndex)))
                list.Add(Item2);
            if (!Item3.Equals(default(FighterIndex)))
                list.Add(Item3);
            if (!Item4.Equals(default(FighterIndex)))
                list.Add(Item4);

            return [.. list];
        }

        public void Set(IEnumerable<FighterIndex> list)
        {
            Item1 = list.ElementAtOrDefault(0);
            Item2 = list.ElementAtOrDefault(1);
            Item3 = list.ElementAtOrDefault(2);
            Item4 = list.ElementAtOrDefault(3);
        }

        public void AddPlayer(FighterIndex index)
        {
            if (Item1.Equals(default(FighterIndex)))
                Item1 = index;
            else if (Item2.Equals(default(FighterIndex)))
                Item2 = index;
            else if (Item3.Equals(default(FighterIndex)))
                Item3 = index;
            else if (Item4.Equals(default(FighterIndex)))
                Item4 = index;
        }

        public void RemovePlayer(FighterIndex index)
        {
            if (Item1.Equals(index))
                Item1 = default;
            else if (Item2.Equals(index))
                Item2 = default;
            else if (Item3.Equals(index))
                Item3 = default;
            else if (Item4.Equals(index))
                Item4 = default;
        }

        public override string ToString()
        {
            StringBuilder output = new();

            if (!Item1.Equals(default(Team)))
                output.Append(Item1 + ", ");
            if (!Item2.Equals(default(Team)))
                output.Append(Item2 + ", ");
            if (!Item3.Equals(default(Team)))
                output.Append(Item3 + ", ");
            if (!Item4.Equals(default(Team)))
                output.Append(Item4 + ", ");

            return output.ToString();
        }
    }
}

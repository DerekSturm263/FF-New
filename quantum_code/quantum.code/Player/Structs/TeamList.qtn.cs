using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum
{
    public unsafe partial struct TeamList
    {
        public IEnumerable<Team> Get(Frame f)
        {
            List<Team> list = [];

            if (!Item1.Equals(default(Team)))
                list.Add(Item1);
            if (!Item2.Equals(default(Team)))
                list.Add(Item2);
            if (!Item3.Equals(default(Team)))
                list.Add(Item3);
            if (!Item4.Equals(default(Team)))
                list.Add(Item4);

            return [.. list];
        }

        public void Set(IEnumerable<Team> list)
        {
            Item1 = list.ElementAtOrDefault(0);
            Item2 = list.ElementAtOrDefault(1);
            Item3 = list.ElementAtOrDefault(2);
            Item4 = list.ElementAtOrDefault(3);
        }

        public void AddTeam(Team team)
        {
            if (Item1.Equals(default(Team)))
                Item1 = team;
            else if (Item2.Equals(default(Team)))
                Item2 = team;
            else if (Item3.Equals(default(Team)))
                Item3 = team;
            else if (Item4.Equals(default(Team)))
                Item4 = team;
        }

        public void RemoveTeam(Team team)
        {
            if (Item1.Equals(team))
                Item1 = default;
            else if (Item2.Equals(team))
                Item2 = default;
            else if (Item3.Equals(team))
                Item3 = default;
            else if (Item4.Equals(team))
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

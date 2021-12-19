using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AoCBase;

namespace AoC2021.Solvers
{
    public class Solver06 : BaseSolver
    {
        private Dictionary<int, BigInteger> initialFishState;

        public Solver06()
        {
            initialFishState = new Dictionary<int,BigInteger>();
            foreach (var fish in InputReader<int>().ReadInputAsSingleCommaSeparatedLine())
            {
                initialFishState.AddToValue(fish, 1);
            }
        }

        public override string Solve_1()
        {
            var total = GetFishCountAfterDays(80);
            return total.ToString();
        }

        public override string Solve_2()
        {
            var total = GetFishCountAfterDays(256);
            return total.ToString();
        }

        private BigInteger GetFishCountAfterDays(int days)
        {
            var d = 0;
            var fish = initialFishState;
            while (d < days)
            {
                fish = GetNextDay(fish);
                d++;
            }
            return fish.Values.Aggregate((curr, next) => curr + next);
        }

        private Dictionary<int, BigInteger> GetNextDay(Dictionary<int, BigInteger> current)
        {
            var next = new Dictionary<int, BigInteger>();
            var i = 0;
            while (i < 8)
            {
                next.Add(i, current.ReadWithDefault(i + 1, 0));
                i++;
            }
            next.AddToValue(6, current.ReadWithDefault(0, 0));
            next.AddToValue(8, current.ReadWithDefault(0, 0));
            return next;
        }
    }
}

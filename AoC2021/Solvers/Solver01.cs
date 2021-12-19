using AoCBase;
using System.Collections.Generic;
using System.Linq;

namespace AoC2021.Solvers
{
    public class Solver01 : BaseSolver
    {
        private List<int> depths;

        public Solver01()
        {
            depths = InputReader<int>().ReadInputAsLines().ToList();
        }

        public override string Solve_1()
        {
            var increases = 0;
            var previous = int.MaxValue;
            foreach (var depth in depths)
            {
                if (depth > previous)
                {
                    increases++;
                }
                previous = depth;
            }
            return increases.ToString();
        }

        public override string Solve_2()
        {
            var increases = 0;

            var i = 3;
            while (i < depths.Count)
            {
                // d_1 + d_2 + d_3 > d_0 + d_1 + d_2 <=> d_3 > d_0
                if (depths[i] > depths[i - 3])
                {
                    increases++;
                }
                i++;
            }
            return increases.ToString();
        }
    }
}

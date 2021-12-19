using System;
using System.Collections.Generic;
using System.Linq;
using AoCBase;

namespace AoC2021.Solvers
{
    public class Solver07 : BaseSolver
    {
        private List<int> CrabPositions;

        public Solver07()
        {
            CrabPositions = InputReader<int>().ReadInputAsSingleCommaSeparatedLine();
        }

        public override string Solve_1()
        {
            var sorted = CrabPositions.OrderBy(x => x).ToList();
            var alignmentPosition = sorted[(sorted.Count / 2)];
            var fuel = sorted.Aggregate(0, (current, next) => current + Math.Abs(next - alignmentPosition));
            return fuel.ToString();
        }

        public override string Solve_2()
        {
            var mean = CrabPositions.Sum() / (decimal) CrabPositions.Count;
            var toCheck = new int[] { (int) Math.Floor(mean), (int) Math.Ceiling(mean) };
            var fuel = toCheck.Select(a => CrabPositions.Aggregate(0, (current, next) => current + GetFuel2(next - a))).Min();
            return fuel.ToString();
        }

        private static int GetFuel2(int diff)
        {
            var distance = Math.Abs(diff);
            return ((distance + 1) * (distance)) / 2;
        }
    }
}

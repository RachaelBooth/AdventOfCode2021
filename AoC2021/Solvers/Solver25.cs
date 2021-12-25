using AoCBase;
using AoCBase.Vectors;
using System.Collections.Generic;
using System.Linq;

namespace AoC2021.Solvers
{
    public class Solver25 : BaseSolver
    {
        private HashSet<(int x, int y)> InitialEastFacingHerd;
        private HashSet<(int x, int y)> InitialSouthFacingHerd;
        private int maxX;
        private int maxY;

        public Solver25()
        {
            var input = InputReader().Parse2DimensionalGrid();
            InitialEastFacingHerd = new HashSet<(int x, int y)>();
            InitialSouthFacingHerd= new HashSet<(int x, int y)>();
            maxX = input.Keys.Max(k => k.x);
            maxY = input.Keys.Max(k => k.y);

            foreach (var kv in input)
            {
                if (kv.Value == '>')
                {
                    InitialEastFacingHerd.Add(kv.Key);
                }
                else if (kv.Value == 'v')
                {
                    InitialSouthFacingHerd.Add(kv.Key);
                }
            }
        }

        public override string Solve_1()
        {
            var steps = 0;
            var current = ((HashSet<(int x, int y)> east, HashSet<(int x, int y)> south)) (InitialEastFacingHerd, InitialSouthFacingHerd);
            while (true)
            {
                steps++;
                var afterMove = Step(current);

                if (afterMove.east.SetEquals(current.east) && afterMove.south.SetEquals(current.south))
                {
                    // Nothing moved, since impossible for different sea cucumber to be in the same position that one in the same herd was last time
                    // (So can't be the same set of locations but swapping which cucumber is in each)
                    return steps.ToString();
                }

                current = afterMove;
            }
        }

        private (HashSet<(int x, int y)> east, HashSet<(int x, int y)> south) Step((HashSet<(int x, int y)> east, HashSet<(int x, int y)> south) current)
        {
            var east = new HashSet<(int x, int y)>();
            var south = new HashSet<(int x, int y)>();
            foreach (var cucumber in current.east)
            {
                var potentialNext = cucumber.x != maxX ? cucumber.Plus((1, 0)) : (0, cucumber.y);
                if (current.east.Contains(potentialNext) || current.south.Contains(potentialNext))
                {
                    east.Add(cucumber);
                }
                else
                {
                    east.Add(potentialNext);
                }
            }
            foreach (var cucumber in current.south)
            {
                var potentialNext = cucumber.y != maxY ? cucumber.Plus((0, 1)) : (cucumber.x, 0);
                if (east.Contains(potentialNext) || current.south.Contains(potentialNext))
                {
                    south.Add(cucumber);
                }
                else
                {
                    south.Add(potentialNext);
                }
            }
            return (east, south);
        }

        public override string Solve_2()
        {
            return "Merry Christmas";
        }
    }
}

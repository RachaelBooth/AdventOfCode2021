using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AoCBase;
using AoCBase.Vectors;

namespace AoC2021.Solvers
{
    public class Solver11 : BaseSolver
    {
        private GridMap<int> initialState;

        public Solver11()
        {
            initialState = new GridMap<int>(InputReader().Parse2DimensionalGrid<int>(c => int.Parse(c.ToString())));
        }

        public override string Solve_1()
        {
            var result = Evolve(100, initialState, m => false);
            return result.flashCount.ToString();
        }

        public override string Solve_2()
        {
            // Should be enough steps to always have exited early from all flashing.
            // If answer is 10000, be suspicious
            var result = Evolve(10000, initialState, m => m.FindAll(0).Count == 100);
            return result.stepsRun.ToString();
        }

        private (GridMap<int> map, BigInteger flashCount, BigInteger stepsRun) Evolve(BigInteger steps, GridMap<int> startState, Func<GridMap<int>, bool> returnImmediately)
        {
            var seen = new List<(GridMap<int> map, BigInteger flashCount, BigInteger steps)>();
            seen.Add((startState, 0, 0));

            BigInteger s = 0;
            var currentState = ((GridMap<int> map, BigInteger flashCount)) (startState, 0);
            while (s < steps)
            {
                var next = NextState(currentState);
                s++;
                if (next.map.FindAll(0).Count == 100)
                {
                    return (next.map, next.flashCount, s);
                }
                // Could probably tidy this up now and track steps in state, but meh
                var prev = seen.Where(state => state.map.Matches(next.map));
                if (prev.Any())
                {
                    var initial = prev.First();
                    var loopLength = s - initial.steps;
                    var remaining = ModularArithmetic.NonNegativeMod(steps - s, loopLength);
                    var st = initial.steps + remaining;
                    var result = seen.First(state => state.steps == st);
                    var map = result.map.Copy();
                    var numberOfLoops = (steps - s - remaining) / loopLength;
                    var flashCount = (next.flashCount - initial.flashCount) * numberOfLoops + result.flashCount - initial.flashCount;
                    return (map, flashCount, s);
                }

                seen.Add((next.map, next.flashCount, s));
                currentState = next;
            }
            return (currentState.map, currentState.flashCount, s);
        }

        private (GridMap<int> map, BigInteger flashCount) NextState((GridMap<int> map, BigInteger flashCount) current)
        {
            var next = current.map.Copy();
            next.Update(i => i + 1);
            
            var flashed = new HashSet<(int x, int y)>();
            var nines = next.LocationsWhere(l => next.ReadWithDefault(l, false) > 9).ToList();
            while (nines.Any())
            {
                foreach (var nine in nines)
                {
                    flashed.Add(nine);
                    foreach (var neighbour in nine.NeighbouringLocations())
                    {
                        next.UpdateIfExists(neighbour, i => i + 1);
                    }
                }
                nines = next.LocationsWhere(l => next.ReadWithDefault(l, false) > 9).Where(l => !flashed.Contains(l)).ToList();
            }

            foreach (var location in flashed)
            {
                next.UpdateIfExists(location, i => 0);
            }
            return (next, current.flashCount + flashed.Count);
        }
    }
}

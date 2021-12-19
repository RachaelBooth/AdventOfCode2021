using System;
using System.Collections.Generic;
using System.Linq;
using AoCBase;
using AoCBase.Vectors;

namespace AoC2021.Solvers
{
    public class Solver15 : BaseSolver
    {
        private readonly Dictionary<(int x, int y), int> RiskMap;
        private int MaxX;
        private int MaxY;
        private readonly Dictionary<(int x, int y), int?> Part2RiskMap;

        public Solver15()
        {
            RiskMap = InputReader().Parse2DimensionalGrid(c => int.Parse(c.ToString()));
            MaxX = RiskMap.Keys.Select(l => l.x).Max();
            MaxY = RiskMap.Keys.Select(l => l.y).Max();
            Part2RiskMap = new Dictionary<(int x, int y), int?>();
        }

        public override string Solve_1()
        {
            var start = (0, 0);
            var end = (MaxX, MaxY);
            var result = GetLowestRiskLevel(start, end, l => RiskMap.ContainsKey(l) ? RiskMap[l] : null);
            return result.ToString();
        }

        public override string Solve_2()
        {
            var start = (0, 0);
            var end = (5 * (MaxX + 1) - 1, 5 * (MaxY + 1) - 1);
            var result = GetLowestRiskLevel(start, end, l => GetPart2RiskLevel(l));
            return result.ToString();
        }

        private int? GetPart2RiskLevel((int x, int y) location)
        {
            if (Part2RiskMap.ContainsKey(location))
            {
                return Part2RiskMap[location];
            }

            if (RiskMap.ContainsKey(location))
            {
                var risk = RiskMap[location];
                Part2RiskMap.Add(location, risk);
                return risk;
            }

            if (location.x < 0 || location.y < 0 || location.x > 5 * (MaxX + 1) - 1 || location.y > 5 * (MaxY + 1) - 1)
            {
                Part2RiskMap.Add(location, null);
                return null;
            }

            var increment = 0;
            var x = location.x;
            var y = location.y;
            while (x > MaxX)
            {
                x = x - MaxX - 1;
                increment++;
            }
            while (y > MaxY)
            {
                y = y - MaxY - 1;
                increment++;
            }
            var res = (int)ModularArithmetic.NonNegativeMod(RiskMap[(x, y)] + increment - 1, 9) + 1;
            Part2RiskMap.Add(location, res);
            return res;
        }

        private int GetLowestRiskLevel((int x, int y) start, (int x, int y) end, Func<(int x, int y), int?> getRisk)
        {
            var states = new Dictionary<(int x, int y), int>();
            states.Add(start, 0);
            var seen = new Dictionary<(int x, int y), int>
            {
                { start, 0 }
            };
            while (states.Any())
            {
                var newStates = new Dictionary<(int x, int y), int>();
                foreach (var state in states)
                {
                    foreach (var neighbour in state.Key.NeighbouringLocationsWithoutDiagonals())
                    {
                        var nRisk = getRisk(neighbour);
                        if (nRisk.HasValue)
                        {
                            var newRisk = state.Value + nRisk.Value;
                            if (!seen.ContainsKey(end) || newRisk < seen[end])
                            {
                                if (!seen.ContainsKey(neighbour))
                                {
                                    seen.Add(neighbour, newRisk);
                                    newStates.Add(neighbour, newRisk);
                                }
                                else
                                {
                                    if (seen[neighbour] > newRisk)
                                    {
                                        seen[neighbour] = newRisk;
                                        if (newStates.ContainsKey(neighbour))
                                        {
                                            newStates[neighbour] = newRisk;
                                        }
                                        else
                                        {
                                            newStates.Add(neighbour, newRisk);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                states = newStates;
            }

            return seen[end];
        }
    }
}

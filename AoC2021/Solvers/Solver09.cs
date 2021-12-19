using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AoCBase;
using AoCBase.Vectors;

namespace AoC2021.Solvers
{
    public class Solver09 : BaseSolver
    {
        private GridMap<int> heightmap;

        public Solver09()
        {
            heightmap = new GridMap<int>(InputReader().Parse2DimensionalGrid(c => int.Parse(c.ToString())), int.MaxValue);
        }

        public override string Solve_1()
        {
            var lowPoints = heightmap.ValuesWhere(location => location.NeighbouringLocationsWithoutDiagonals().All(l => heightmap.ReadWithDefault(l, false) > heightmap.ReadWithDefault(location, false)));
            var riskLevel = lowPoints.Aggregate(0, (current, next) => current + next + 1);
            return riskLevel.ToString();
        }

        public override string Solve_2()
        {
            var lowPoints = heightmap.LocationsWhere(location => location.NeighbouringLocationsWithoutDiagonals().All(l => heightmap.ReadWithDefault(l, false) > heightmap.ReadWithDefault(location, false)));
            var basinSizes = lowPoints.Select(l => BasinSize(l)).OrderByDescending(s => s).ToList();
            var result = basinSizes[0] * basinSizes[1] * basinSizes[2];
            return result.ToString();
        }

        private int BasinSize((int x, int y) lowPoint)
        {
            var basin = new HashSet<(int x, int y)> { lowPoint };
            var edges = new HashSet<(int x, int y)> { lowPoint };
            while (edges.Any())
            {
                var l = new HashSet<(int x, int y)>();
                foreach (var edge in edges)
                {
                    var edgeHeight = heightmap.ReadWithDefault(edge);
                    foreach (var neighbour in edge.NeighbouringLocationsWithoutDiagonals())
                    {
                        var height = heightmap.ReadWithDefault(neighbour);
                        if (height < 9 && height > edgeHeight)
                        {
                            l.Add(neighbour);
                        }
                    }
                }
                edges = l.Where(x => !basin.Contains(x)).ToHashSet();
                basin.UnionWith(l);
            }
            return basin.Count;
        }
    }
}

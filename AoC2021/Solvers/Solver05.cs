using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AoCBase;

namespace AoC2021.Solvers
{
    public class Solver05 : BaseSolver
    {
        private Dictionary<Vector2, Vector2> ventCounts;

        public Solver05()
        {
            var lines = InputReader<LineSegment>().ReadInputAsLines();
            ventCounts = new Dictionary<Vector2, Vector2>();
            foreach (var line in lines)
            {
                foreach (var point in line.GetPoints())
                {
                    ventCounts.AddToValue(point, new Vector2(line.IsDiagonal() ? 0 : 1, 1));
                }
            }
        }

        public override string Solve_1()
        {
            var overlapCount = ventCounts.Values.Count(v => v.X > 1);
            return overlapCount.ToString();
        }

        public override string Solve_2()
        {
            var overlapCount = ventCounts.Values.Count(v => v.Y > 1);
            return overlapCount.ToString();
        }

        private class LineSegment
        {
            private Vector2 start;
            private Vector2 end;

            public LineSegment(Vector2 start, Vector2 end)
            {
                this.start = start;
                this.end = end;
            }

            public bool IsDiagonal()
            {
                return (start.X != end.X) && (start.Y != end.Y);
            }

            public IEnumerable<Vector2> GetPoints()
            {
                var length = (int) Math.Max(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)) + 1;
                var diffX = end.X == start.X ? 0 : end.X > start.X ? 1 : -1;
                var diffY = end.Y == start.Y ? 0 : end.Y > start.Y ? 1 : -1;
                var diffVector = new Vector2(diffX, diffY);
                return Enumerable.Range(0, length).Select(i => start + i * diffVector);
            }

            public static LineSegment Parse(string line)
            {
                var parts = line.Split(" -> ").SelectMany(p => p.Split(',').Select(i => int.Parse(i))).ToList();
                var start = new Vector2(parts[0], parts[1]);
                var end = new Vector2(parts[2], parts[3]);
                return new LineSegment(start, end);
            }
        }
    }
}

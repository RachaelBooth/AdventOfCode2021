using System;
using System.Collections.Generic;
using System.Linq;
using AoCBase;
using AoCBase.Vectors;

namespace AoC2021.Solvers
{
    public class Solver17 : BaseSolver
    {
        private ((int min, int max) x, (int min, int max) y) TargetArea;

        public Solver17()
        {
            TargetArea = ((175, 227), (-134, -79));
        }

        public override string Solve_1()
        {
            var hits = new List<((int x, int y) initialVelocity, (int x, int y) pos, int maxY)>();
            var test = ProbeTrajectory((20, 133));
            var y = 0;
            while (y <= 134)
            {
                var x = 16;
                while (x <= 227)
                {
                    var trajectory = ProbeTrajectory((x, y));
                    if (IsInTarget(trajectory[^1]))
                    {
                        hits.Add(((x, y), trajectory[^1], trajectory.Max(p => p.y)));
                    }
                    x++;
                }
                y++;
            }

            if (!hits.Any())
            {
                throw new Exception("oh dear");
            }

            var result = hits.Max(h => h.maxY);
            return result.ToString();
        }

        public override string Solve_2()
        {
            var hits = new List<((int x, int y) initialVelocity, (int x, int y) pos, int maxY)>();
            var test = ProbeTrajectory((20, 133));
            var y = -134;
            while (y <= 134)
            {
                var x = 16;
                while (x <= 227)
                {
                    var trajectory = ProbeTrajectory((x, y));
                    if (IsInTarget(trajectory[^1]))
                    {
                        hits.Add(((x, y), trajectory[^1], trajectory.Max(p => p.y)));
                    }
                    x++;
                }
                y++;
            }

            if (!hits.Any())
            {
                throw new Exception("oh dear");
            }

            var result = hits.Count;
            return result.ToString();
        }

        private bool IsInTarget((int x, int y) position)
        {
            return position.x >= TargetArea.x.min && position.x <= TargetArea.x.max && position.y >= TargetArea.y.min && position.y <= TargetArea.y.max;
        }

        private List<(int x, int y)> ProbeTrajectory((int x, int y) initialVelocity)
        {
            if (initialVelocity.x <= 0) {
                throw new Exception("This won't work");
            }
            var trajectory = new List<(int x, int y)>();
            trajectory.Add((0, 0));
            var current = ((int x, int y)) (0, 0);
            var velocity = initialVelocity;
            while (current.x <= TargetArea.x.max && current.y >= TargetArea.y.min)
            {
                current = current.Plus(velocity);
                velocity = velocity.Minus((velocity.x > 0 ? 1 : 0, 1));
                trajectory.Add(current);
                if (IsInTarget(current))
                {
                    // In TargetArea
                    return trajectory;
                }
                if (velocity.x == 0 && current.x < TargetArea.x.min)
                {
                    // No longer moving horizontally, and not reached target area
                    return trajectory;
                }
            }
            return trajectory;
        }
    }
}

using AoCBase;
using AoCBase.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoC2021.Solvers
{
    public class Solver22 : BaseSolver
    {
        private List<RebootStep> steps;
        private List<Region> regionsOn;

        public Solver22()
        {
            steps = InputReader<RebootStep>().ReadInputAsLines().ToList();
            regionsOn = RunInitialization();
        }

        public override string Solve_1()
        {
            var result = regionsOn.Aggregate(BigInteger.Zero, (current, next) => current + next.SizeOfOverlap(new Region((-50, -50, -50), (50, 50, 50))));
            return result.ToString();
        }

        public override string Solve_2()
        {
            var result = regionsOn.Aggregate(BigInteger.Zero, (current, next) => current + next.Size());
            return result.ToString();
        }

        private List<Region> RunInitialization()
        {
            var on = new List<Region>();
            foreach (var step in steps)
            {
                if (step.type == RebootStep.StepType.On)
                {
                    var overlaps = on.Select(r => r.Overlap(step.region)).Where(o => o != null).Select(r => r.Value).ToList();
                    if (!overlaps.Any())
                    {
                        on.Add(step.region);
                    }
                    else
                    {
                        on.AddRange(step.region.Excluding(overlaps));
                    }
                }
                else
                {
                    on = on.SelectMany(r => r.Excluding(new List<Region> { step.region })).ToList();
                }
            }
            return on;
        }

        private record struct Region((int x, int y, int z) Min, (int x, int y, int z) Max) 
        {
            public BigInteger Size()
            {
                var diffs = Max.Minus(Min).Plus((1, 1, 1));
                // VS lies - need to be big integers during the multiplication
                return ((BigInteger) Math.Abs(diffs.x)) * ((BigInteger) Math.Abs(diffs.y)) * ((BigInteger) Math.Abs(diffs.z));
            }

            public BigInteger SizeOfOverlap(Region other)
            {
                var overlap = Overlap(other);
                if (!overlap.HasValue)
                {
                    return 0;
                }
                return overlap.Value.Size();
            }

            public Region? Overlap(Region other)
            {
                if (other.Max.x < Min.x || Max.x < other.Min.x)
                {
                    return null;
                }
                if (other.Max.y < Min.y || Max.y < other.Min.y)
                {
                    return null;
                }
                if (other.Max.z < Min.z || Max.z < other.Min.z)
                {
                    return null;
                }

                var overlapMin = (Math.Max(Min.x, other.Min.x), Math.Max(Min.y, other.Min.y), Math.Max(Min.z, other.Min.z));
                var overlapMax = (Math.Min(Max.x, other.Max.x), Math.Min(Max.y, other.Max.y), Math.Min(Max.z, other.Max.z));
                return new Region(overlapMin, overlapMax);
            }

            public List<Region> Excluding(List<Region> others)
            {
                var result = new List<Region> { this };
                foreach (var other in others)
                {
                    result = result.SelectMany(r =>
                    {
                        var overlap = r.Overlap(other);
                        if (!overlap.HasValue)
                        {
                            return new Region[] { r };
                        }

                        if (overlap == r)
                        {
                            return Array.Empty<Region>();
                        }

                        var split = new List<Region>();
                        if (overlap.Value.Min.x > r.Min.x)
                        {
                            split.Add(new Region(r.Min, (overlap.Value.Min.x - 1, r.Max.y, r.Max.z)));
                        }
                        if (overlap.Value.Max.x < r.Max.x)
                        {
                            split.Add(new Region((overlap.Value.Max.x + 1, r.Min.y, r.Min.z), r.Max));
                        }
                        if (overlap.Value.Min.y > r.Min.y)
                        {
                            split.Add(new Region((overlap.Value.Min.x, r.Min.y, r.Min.z), (overlap.Value.Max.x, overlap.Value.Min.y - 1, r.Max.z)));
                        }
                        if (overlap.Value.Max.y < r.Max.y)
                        {
                            split.Add(new Region((overlap.Value.Min.x, overlap.Value.Max.y + 1, r.Min.z), (overlap.Value.Max.x, r.Max.y, r.Max.z)));
                        }
                        if (overlap.Value.Min.z > r.Min.z)
                        {
                            split.Add(new Region((overlap.Value.Min.x, overlap.Value.Min.y, r.Min.z), (overlap.Value.Max.x, overlap.Value.Max.y, overlap.Value.Min.z - 1)));
                        }
                        if (overlap.Value.Max.z < r.Max.z)
                        {
                            split.Add(new Region((overlap.Value.Min.x, overlap.Value.Min.y, overlap.Value.Max.z + 1), (overlap.Value.Max.x, overlap.Value.Max.y, r.Max.z)));
                        }
                        return split.ToArray();
                    }).ToList();
                }
                return result;
            }
        }

        private class RebootStep
        {
            public Region region;
            public StepType type;

            public RebootStep(StepType type, int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
            {
                this.type = type;
                region = new Region((minX, minY, minZ), (maxX, maxY, maxZ));
            }

            public enum StepType
            {
                On,
                Off
            }

            public static RebootStep Parse(string line)
            {
                var parts = line.Split(' ', ',', '=', '.');
                var type = parts[0] == "on" ? StepType.On : StepType.Off;
                return new RebootStep(type, int.Parse(parts[2]), int.Parse(parts[4]), int.Parse(parts[6]), int.Parse(parts[8]), int.Parse(parts[10]), int.Parse(parts[12]));
            }
        }
    }
}

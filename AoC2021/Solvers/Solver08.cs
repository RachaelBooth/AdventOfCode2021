using System;
using System.Collections.Generic;
using System.Linq;
using AoCBase;

namespace AoC2021.Solvers
{
    public class Solver08 : BaseSolver
    {
        private List<SegmentDisplay> displays;

        public Solver08()
        {
            displays = InputReader<SegmentDisplay>().ReadInputAsLines().ToList();
        }

        public override string Solve_1()
        {
            return displays.Aggregate(0, (current, next) => current + next.output.Count(d => d.IsOne() || d.IsFour() || d.IsSeven() || d.IsEight())).ToString();
        }

        public override string Solve_2()
        {
            return displays.Aggregate(0, (current, next) => current + next.DecodeOutput()).ToString();
        }

        private class SegmentDisplay
        {
            private List<Digit> patternsDisplayed;
            public List<Digit> output;

            public SegmentDisplay(List<Digit> patternsDisplayed, List<Digit> output)
            {
                this.patternsDisplayed = patternsDisplayed;
                this.output = output;
            }

            public int DecodeOutput()
            {
                var segmentMapping = new Dictionary<char, List<char>>();
                var keys = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' };
                foreach (var key in keys)
                {
                    segmentMapping.Add(key, new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G' });
                }

                var one = patternsDisplayed.Where(o => o.IsOne()).ToList();
                if (one.Count > 0)
                {
                    var oneSegments = one[0].Segments();
                    foreach (var segment in keys)
                    {
                        if (oneSegments.Contains(segment))
                        {
                            segmentMapping.RestrictTo(segment, 'C', 'F');
                        }
                        else
                        {
                            segmentMapping.Exclude(segment, 'C', 'F');
                        }
                    }
                }

                var four = patternsDisplayed.Where(o => o.IsFour()).ToList();
                if (four.Count > 0)
                {
                    var fourSegments = four[0].Segments();
                    foreach (var segment in keys)
                    {
                        if (fourSegments.Contains(segment))
                        {
                            segmentMapping.RestrictTo(segment, 'B', 'C', 'D', 'F');
                        }
                        else
                        {
                            segmentMapping.Exclude(segment, 'B', 'C', 'D', 'F');
                        }
                    }
                }

                var seven = patternsDisplayed.Where(o => o.IsSeven()).ToList();
                if (seven.Count > 0)
                {
                    var sevenSegments = seven[0].Segments();
                    foreach (var segment in keys)
                    {
                        if (sevenSegments.Contains(segment))
                        {
                            segmentMapping.RestrictTo(segment, 'A', 'C', 'F');
                        }
                        else
                        {
                            segmentMapping.Exclude(segment, 'A', 'C', 'F');
                        }
                    }
                }

                // 1, 4, 7 above
                // 8 is everything

                // 2: A_CDE_G
                // 3: A_CD_FG
                // 5: AB_D_FG
                var five_segs = patternsDisplayed.Where(o => o.CountSegments() == 5).ToList();
                if (five_segs.Count > 0)
                {
                    foreach (var s in five_segs)
                    {
                        var segs = s.Segments();
                        foreach (var segment in keys)
                        {
                            if (!segs.Contains(segment))
                            {
                                // Only B C E and F are excluded from any five segment digit
                                segmentMapping.RestrictTo(segment, 'B', 'C', 'E', 'F');
                            }
                        }
                    }
                }

                // 0: ABC_EFG
                // 6: AB_DEFG
                // 9: ABCD_FG
                var six_segs = patternsDisplayed.Where(o => o.CountSegments() == 6).ToList();
                if (six_segs.Count > 0)
                {
                    foreach (var s in six_segs)
                    {
                        var segs = s.Segments();
                        foreach (var segment in keys)
                        {
                            if (!segs.Contains(segment))
                            {
                                // Only C D and E are excluded from any six segment digit
                                segmentMapping.RestrictTo(segment, 'C', 'D', 'E');
                            }
                        }
                    }
                }

                while (segmentMapping.Any(kv => kv.Value.Count > 1))
                {
                    var known = segmentMapping.Where(kv => kv.Value.Count == 1).ToList();
                    if (!known.Any())
                    {
                        throw new Exception("Oh dear, think harder");
                    }

                    foreach (var k in known)
                    {
                        var val = segmentMapping[k.Key][0];
                        foreach (var key in keys)
                        {
                            if (key != k.Key)
                            {
                                segmentMapping.Exclude(key, val);
                            }
                        }
                    }

                    if (segmentMapping.Any(kv => kv.Value.Count == 0))
                    {
                        throw new Exception("Excluded too much, think again");
                    }
                }

                var result = "";
                foreach (var digit in output)
                {
                    var segments = string.Join("", digit.Segments().Select(s => segmentMapping[s][0]).OrderBy(c => c));
                    switch (segments)
                    {
                        case "ABCEFG":
                            result += "0";
                            break;
                        case "CF":
                            result += "1";
                            break;
                        case "ACDEG":
                            result += "2";
                            break;
                        case "ACDFG":
                            result += "3";
                            break;
                        case "BCDF":
                            result += "4";
                            break;
                        case "ABDFG":
                            result += "5";
                            break;
                        case "ABDEFG":
                            result += "6";
                            break;
                        case "ACF":
                            result += "7";
                            break;
                        case "ABCDEFG":
                            result += "8";
                            break;
                        case "ABCDFG":
                            result += "9";
                            break;
                    }
                }

                return int.Parse(result);
            }

            public static SegmentDisplay Parse(string line)
            {
                var parts = line.Split(" | ");
                var patterns = parts[0].Split(" ").Select(p => new Digit(p)).ToList();
                var output = parts[1].Split(" ").Select(p => new Digit(p)).ToList();
                return new SegmentDisplay(patterns, output);
            }
        }

        private class Digit
        {
            private readonly Dictionary<char, bool> segmentsOn;
            private readonly char[] segments = { 'a', 'b', 'c', 'd', 'e', 'f', 'g' };

            public Digit(string signalPattern)
            {
                segmentsOn = new Dictionary<char, bool>();
                foreach (var c in segments)
                {
                    segmentsOn.Add(c, signalPattern.Contains(c));
                }
            }

            public bool IsOne()
            {
                return CountSegments() == 2;
            }

            public bool IsFour()
            {
                return CountSegments() == 4;
            }

            public bool IsSeven()
            {
                return CountSegments() == 3;
            }

            public bool IsEight()
            {
                return CountSegments() == 7;
            }

            public List<char> Segments()
            {
                return segmentsOn.Where(s => s.Value).Select(s => s.Key).ToList();
            }

            public int CountSegments()
            {
                return segmentsOn.Count(s => s.Value);
            }
        }
    }
}

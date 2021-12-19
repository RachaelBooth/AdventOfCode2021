using AoCBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC2021.Solvers
{
    public class Solver03 : BaseSolver
    {
        private readonly List<string> diagnosticReportLines;

        public Solver03()
        {
            diagnosticReportLines = InputReader<string>().ReadInputAsLines().ToList();
        }

        public override string Solve_1()
        {
            var mostCommon = diagnosticReportLines[0].Select((c, i) => MostCommonAtPosition(i));
            var gammaRate = IntFromBinaryEnumerable(mostCommon);
            var epsilonRate = IntFromBinaryEnumerable(mostCommon.Select(i => 1 - i));
            var powerConsumption = gammaRate * epsilonRate;
            return powerConsumption.ToString();
        }

        public override string Solve_2()
        {
            var oxLines = diagnosticReportLines.ToList();
            var index = 0;
            while (oxLines.Count > 1)
            {
                var toKeep = MostCommonAtPosition(index, oxLines);
                oxLines = oxLines.Where(l => int.Parse(l[index].ToString()) == toKeep).ToList();
                index++;
            }
            var oxygenGeneratorRating = Convert.ToInt32(oxLines[0], 2);

            var coLines = diagnosticReportLines.ToList();
            index = 0;
            while (coLines.Count > 1)
            {
                var toKeep = 1 - MostCommonAtPosition(index, coLines);
                coLines = coLines.Where(l => int.Parse(l[index].ToString()) == toKeep).ToList();
                index++;
            }
            var co2ScrubberRating = Convert.ToInt32(coLines[0], 2);

            var lifeSupportRating = oxygenGeneratorRating * co2ScrubberRating;
            return lifeSupportRating.ToString();
        }

        private int MostCommonAtPosition(int index)
        {
            return MostCommonAtPosition(index, diagnosticReportLines);
        }

        private static int MostCommonAtPosition(int index, List<string> lines)
        {
            var ones = lines.Count(l => l[index] == '1');
            var zeros = lines.Count - ones;
            // Always bias towards 1 - never occurs in part 1, in part 2 both want 1 to be considered 'more' (co2 uses the less common)
            return ones >= zeros ? 1 : 0;
        }

        private static int IntFromBinaryEnumerable(IEnumerable<int> values)
        {
            var s = string.Join("", values);
            return Convert.ToInt32(s, 2);
        }
    }
}

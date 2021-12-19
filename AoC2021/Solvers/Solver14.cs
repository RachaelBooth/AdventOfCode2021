using System.Collections.Generic;
using System.Linq;
using AoCBase;

namespace AoC2021.Solvers
{
    public class Solver14 : BaseSolver
    {
        private readonly string Template;
        private readonly Dictionary<(char First, char Second), List<(char First, char Second)>> Rules;
        Dictionary<(char first, char second), long> TemplatePairs;

        public Solver14()
        {
            var input = InputReader<string, PairInsertionRule>().ReadAsMultipartLines();
            Template = input.Item1[0];
            Rules = new Dictionary<(char First, char Second), List<(char First, char Second)>>();
            foreach (var rule in input.Item2)
            {
                Rules.AddOptions((rule.First, rule.Second), (rule.First, rule.Insert), (rule.Insert, rule.Second));
            }
            TemplatePairs = new Dictionary<(char First, char Second), long>();
            var i = 0;
            while (i < Template.Length - 1)
            {
                TemplatePairs.AddToValue((Template[i], Template[i + 1]), 1);
                i++;
            }
        }

        public override string Solve_1()
        {
            var expanded = Expand(TemplatePairs, 10);
            var result = GetMostToLeastAppearancesDiff(expanded);
            return result.ToString();
        }

        public override string Solve_2()
        {
            var expanded = Expand(TemplatePairs, 40);
            var result = GetMostToLeastAppearancesDiff(expanded);
            return result.ToString();
        }

        private long GetMostToLeastAppearancesDiff(Dictionary<(char first, char second), long> pairs)
        {
            var elementCounts = new Dictionary<char, long>();
            foreach (var pair in pairs)
            {
                // Double counts everything except first and last character
                elementCounts.AddToValue(pair.Key.first, pair.Value);
                elementCounts.AddToValue(pair.Key.second, pair.Value);
            }
            // Double count all the characters
            elementCounts.AddToValue(Template[0], 1);
            elementCounts.AddToValue(Template[^1], 1);
            // Still doubled
            var result = elementCounts.Values.Max() - elementCounts.Values.Min();
            return result / 2;
        }

        private Dictionary<(char first, char second), long> Expand(Dictionary<(char first, char second), long> current, long times)
        {
            var t = 0;
            var c = current;
            while (t < times)
            {
                c = ExpandOnce(c);
                t++;
            }
            return c;
        }

        private Dictionary<(char first, char second), long> ExpandOnce(Dictionary<(char first, char second), long> current)
        {
            var result = new Dictionary<(char first, char second), long>();
            foreach (var pair in current)
            {
                if (Rules.ContainsKey(pair.Key))
                {
                    foreach (var newPair in Rules[pair.Key])
                    {
                        result.AddToValue(newPair, pair.Value);
                    }
                }
                else
                {
                    result.AddToValue(pair.Key, pair.Value);
                }
            }
            return result;
        }

        private record class PairInsertionRule(char First, char Second, char Insert)
        {
            public static PairInsertionRule Parse(string line)
            {
                return new PairInsertionRule(line[0], line[1], line[6]);
            }
        }
    }
}

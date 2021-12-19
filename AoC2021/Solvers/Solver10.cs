using System.Collections.Generic;
using System.Linq;
using AoCBase;

namespace AoC2021.Solvers
{
    public class Solver10 : BaseSolver
    {
        private readonly List<string> lines;
        private readonly List<(LineState state, long score)> lineStates;
        private readonly Dictionary<char, char> bracketPairs;
        private readonly Dictionary<char, long> corruptedScores;
        private readonly Dictionary<char, long> incompleteScores;

        public Solver10()
        {
            lines = InputReader<string>().ReadInputAsLines().ToList();
            bracketPairs = new Dictionary<char, char>
            {
                { ')', '(' },
                { ']', '[' },
                { '}', '{' },
                { '>', '<' }
            };
            corruptedScores = new Dictionary<char, long>
            {
                { ')', 3 },
                { ']', 57 },
                { '}', 1197 },
                { '>', 25137 }
            };
            incompleteScores = new Dictionary<char, long>
            {
                { '(', 1 },
                { '[', 2 },
                { '{', 3 },
                { '<', 4 }
            };
            lineStates = lines.Select(l => GetLineState(l)).ToList();
        }

        public override string Solve_1()
        {
            var score = lineStates.Where(s => s.state == LineState.Corrupted).Sum(s => s.score);
            return score.ToString();
        }

        public override string Solve_2()
        {
            // TODO: Maybe write a median extension method
            var incomplete = lineStates.Where(s => s.state == LineState.Incomplete).OrderBy(s => s.score).ToList();
            var middleScore = incomplete[incomplete.Count / 2];
            return middleScore.score.ToString();
        }

        private enum LineState
        {
            Valid,
            Incomplete,
            Corrupted
        }

        private (LineState state, long score) GetLineState(string line)
        {
            var seen = new Stack<char>();
            foreach (var c in line)
            {
                if (bracketPairs.ContainsKey(c))
                {
                    // Closing bracket
                    if (!seen.Any())
                    {
                        // Don't think we expect this - might be undefined
                        return (LineState.Corrupted, corruptedScores[c]);
                    }

                    var currentOpen = seen.Pop();
                    if (bracketPairs[c] != currentOpen)
                    {
                        return (LineState.Corrupted, corruptedScores[c]);
                    }
                }
                else
                {
                    // We assume that the strings contain no characters that are not brackets given the problem setup
                    seen.Push(c);
                }
            }
            
            if (seen.Any())
            {
                long score = 0;
                while (seen.Any())
                {
                    var c = seen.Pop();
                    score *= 5;
                    score += incompleteScores[c];
                }
                return (LineState.Incomplete, score);
            }

            // Shouldn't get here either
            return (LineState.Valid, 0);
        }
    }
}

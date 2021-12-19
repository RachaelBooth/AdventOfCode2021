using System.Collections.Generic;
using AoCBase;

namespace AoC2021.Solvers
{
    public class Solver13 : BaseSolver
    {
        private List<FoldInstruction> foldInstructions;
        private GridMap<char> paper;

        public Solver13()
        {
            var input = InputReader<DotCoordinate, FoldInstruction>("Extra").ReadAsMultipartLines();
            foldInstructions = input.Item2;
            paper = new GridMap<char>(new Dictionary<(int x, int y), char>(), ' ');
            foreach (var dot in input.Item1)
            {
                paper.Set((dot.X, dot.Y), '#');
            }
        }

        public override string Solve_1()
        {
            var result = Fold(paper, foldInstructions[0]);
            return result.FindAll('#').Count.ToString();
        }

        public override string Solve_2()
        {
            var result = paper;
            foreach (var fold in foldInstructions)
            {
                result = Fold(result, fold);
            }
            result.Draw();
            return "Set breakpoint to see drawing";
        }

        private GridMap<char> Fold(GridMap<char> current, FoldInstruction fold)
        {
            var result = new GridMap<char>(new Dictionary<(int x, int y), char>(), ' ');
            foreach (var dot in current.FindAll('#'))
            {
                if (fold.Coordinate == 'x')
                {
                    var x = dot.x > fold.Value ? (2 * fold.Value) - dot.x : dot.x;
                    result.Set((x, dot.y), '#');
                }
                else
                {
                    var y = dot.y > fold.Value ? (2 * fold.Value) - dot.y : dot.y;
                    result.Set((dot.x, y), '#');
                }
            }
            return result;
        }
        

        private record class DotCoordinate(int X, int Y)
        {
            public static DotCoordinate Parse(string line)
            {
                var parts = line.Split(',');
                return new DotCoordinate(int.Parse(parts[0]), int.Parse(parts[1]));
            }
        }

        private record class FoldInstruction(char Coordinate, int Value)
        {
            public static FoldInstruction Parse(string line)
            {
                var parts = line.Split(' ');
                var equation = parts[2];
                var equationParts = equation.Split('=');
                return new FoldInstruction(equationParts[0][0], int.Parse(equationParts[1]));
            }
        }
    }
}

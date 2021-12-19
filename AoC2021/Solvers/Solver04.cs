using AoCBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC2021.Solvers
{
    public class Solver04 : BaseSolver
    {
        private List<BingoBoard> boards;
        private List<int> numbers;

        public Solver04()
        {
            var data = InputReader<BingoData>().ReadInputAsLineGroups().ToList();
            numbers = data.First(d => d.Type == BingoData.BingoDataType.RandomNumbers).Numbers.ToList();
            boards = data.Where(d => d.Type == BingoData.BingoDataType.Board).Select(d => d.Board).ToList();
        }

        public override string Solve_1()
        {
            foreach (var number in numbers)
            {
                foreach (var board in boards)
                {
                    board.MarkElement(number);
                    if (board.HasWon())
                    {
                        var score = board.SumUnmarked() * number;
                        return score.ToString();
                    }
                }
            }
            return "winning board not found";
        }

        public override string Solve_2()
        {
            var remainingBoards = boards.ToList();
            foreach (var number in numbers)
            {
                foreach (var board in remainingBoards)
                {
                    board.MarkElement(number);
                }
                var unwonBoards = remainingBoards.Where(board => !board.HasWon()).ToList();
                if (!unwonBoards.Any())
                {
                    // We assume there's only one at this point
                    var lastBoard = remainingBoards[0];
                    var score = lastBoard.SumUnmarked() * number;
                    return score.ToString();
                }
                remainingBoards = unwonBoards;
            }
            return "???";
        }

        private class BingoBoard
        {
            private Dictionary<int, (int x, int y)> board;
            private HashSet<(int x, int y)> marked;
            private List<List<(int x, int y)>> potentialLines;


            public BingoBoard(Dictionary<int, (int x, int y)> board)
            {
                this.board = board;
                marked = new HashSet<(int x, int y)>();

                // Assume 5 x 5 boards
                potentialLines = Enumerable.Range(0, 5).Select(x => Enumerable.Range(0, 5).Select(y => (x, y)).ToList()).ToList();
                potentialLines.AddRange(Enumerable.Range(0, 5).Select(y => Enumerable.Range(0, 5).Select(x => (x, y)).ToList()));
                potentialLines.Add(Enumerable.Range(0, 5).Select(i => (i, i)).ToList());
                potentialLines.Add(Enumerable.Range(0, 5).Select(i => (i, 4 - i)).ToList());
            }

            public void MarkElement(int number)
            {
                if (board.ContainsKey(number))
                {
                    marked.Add(board[number]);
                }
            }

            public bool HasWon()
            {
                return potentialLines.Any(line => line.All(l => marked.Contains(l)));
            }

            public int SumUnmarked()
            {
                return board.Where(kv => !marked.Contains(kv.Value)).Select(kv => kv.Key).Sum();
            }
        }

        // TODO: This is deeply unsatisfying. Should update InputReader to handle multiple types somehow.
        private class BingoData
        {
            public enum BingoDataType
            {
                RandomNumbers,
                Board
            }

            public BingoDataType Type;
            public IEnumerable<int> Numbers;
            public BingoBoard Board;

            public BingoData(Dictionary<int, (int x, int y)> board)
            {
                Type = BingoDataType.Board;
                Board = new BingoBoard(board);
            }

            public BingoData(IEnumerable<int> randomNumbers)
            {
                Type = BingoDataType.RandomNumbers;
                Numbers = randomNumbers;
            }

            public static BingoData Parse(List<string> lines)
            {
                if (lines.Count == 1)
                {
                    // Random Numbers
                    return new BingoData(lines[0].Split(',').Select(int.Parse));
                }

                var boardDict = new Dictionary<int, (int x, int y)>();
                var y = 0;
                while (y < lines.Count)
                {
                    var line = lines[y];
                    var values = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    var x = 0;
                    while (x < values.Count)
                    {
                        boardDict.Add(values[x], (x, y));
                        x++;
                    }
                    y++;
                }

                return new BingoData(boardDict);
            }
        }
    }
}

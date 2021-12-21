using AoCBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021.Solvers
{
    public class Solver21 : BaseSolver
    {
        private (int Player1, int Player2) StartingPositions = (2, 1);

        public override string Solve_1()
        {
            var score = new Vector2Int(0, 0);
            var positions = new Vector2Int(StartingPositions.Player1, StartingPositions.Player2);
            var winningCondition = 1000;
            var toPlay = 1;
            var nextDiceRoll = 1;
            var rollCount = 0;
            var boardSize = 10;
            while (score.X < winningCondition && score.Y < winningCondition)
            {
                var move = ModularArithmetic.NonNegativeMod(nextDiceRoll + nextDiceRoll + 1 + nextDiceRoll + 2, boardSize);
                nextDiceRoll = nextDiceRoll + 3;
                rollCount = rollCount + 3;
                if (toPlay == 1)
                {
                    var newOnePos = (int) ModularArithmetic.PositiveMod(positions.X + move, boardSize);
                    score.X += newOnePos;
                    positions.X = newOnePos;
                    toPlay = 2;
                }
                else
                {
                    var newTwoPos = (int) ModularArithmetic.PositiveMod(positions.Y + move, boardSize);
                    score.Y += newTwoPos;
                    positions.Y = newTwoPos;
                    toPlay = 1;
                }
            }
            var loserScore = toPlay == 1 ? score.X : score.Y;
            var result = loserScore * rollCount;
            return result.ToString();
        }

        public override string Solve_2()
        {
            // count of ways of reaching given state
            var states = new Dictionary<(Vector2Int positions, Vector2Int scores, int toPlay), BigInteger>();
            var oneWins = BigInteger.Zero;
            var twoWins = BigInteger.Zero;
            var boardSize = 10;
            var winningCondition = 21;
            var maxDieRoll = 3;

            var threeDieRollOptions = new Dictionary<int, int>();
            var a = 1;
            while (a <= maxDieRoll)
            {
                var b = 1;
                while (b <= maxDieRoll)
                {
                    var c = 1;
                    while (c <= maxDieRoll)
                    {
                        threeDieRollOptions.AddToValue(a + b + c, 1);
                        c++;
                    }
                    b++;
                }
                a++;
            }

            states.Add((new Vector2Int(StartingPositions.Player1, StartingPositions.Player2), new Vector2Int(0, 0), 1), 1);
            var currentScoreToAdvance = 0;
            while (currentScoreToAdvance < 2 * winningCondition)
            {
                var statesToCheck = states.Keys.Where(s => s.scores.X + s.scores.Y == currentScoreToAdvance).ToList();
                foreach (var state in statesToCheck)
                {
                    foreach (var roll in threeDieRollOptions)
                    {
                        var waysToReach = states[state] * roll.Value;
                        if (state.toPlay == 1)
                        {
                            var newOnePos = (int)ModularArithmetic.PositiveMod(state.positions.X + roll.Key, boardSize);
                            var score = state.scores.X + newOnePos;
                            if (score >= winningCondition)
                            {
                                oneWins = oneWins + waysToReach;
                            }
                            else
                            {
                                states.AddToValue((new Vector2Int(newOnePos, state.positions.Y), new Vector2Int(score, state.scores.Y), 2), waysToReach);
                            }
                        }
                        else
                        {
                            var newTwoPos = (int)ModularArithmetic.PositiveMod(state.positions.Y + roll.Key, boardSize);
                            var score = state.scores.Y + newTwoPos;
                            if (score >= winningCondition)
                            {
                                twoWins = twoWins + waysToReach;
                            }
                            else
                            {
                                states.AddToValue((new Vector2Int(state.positions.X, newTwoPos), new Vector2Int(state.scores.X, score), 1), waysToReach);
                            }
                        }
                    }
                }
                currentScoreToAdvance++;
            }

            var result = BigInteger.Max(oneWins, twoWins);
            return result.ToString();
        }
    }
}

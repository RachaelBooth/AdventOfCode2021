using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AoCBase;

namespace AoC2021.Solvers
{
    public class Solver18 : BaseSolver
    {
        private List<string> Numbers;

        public Solver18()
        {
            Numbers = InputReader<string>().ReadInputAsLines().ToList();
        }

        public override string Solve_1()
        {
            var sum = Numbers.Aggregate((current, next) => Add(current, next));
            var result = Magnitude(sum);
            return result.ToString();
        }

        public override string Solve_2()
        {
            var i = 0;
            long maxMag = 0;
            while (i < Numbers.Count)
            {
                var j = 0;
                while (j < Numbers.Count)
                {
                    var sum = Add(Numbers[i], Numbers[j]);
                    var mag = Magnitude(sum);
                    if (mag > maxMag)
                    {
                        maxMag = mag;
                    }
                    j++;
                }
                i++;
            }
            return maxMag.ToString();
        }

        private long Magnitude(string snailfishNumber)
        {
            if (snailfishNumber[0] == '[')
            {
                var i = 0;
                var level = 0;
                while (i < snailfishNumber.Length)
                {
                    if (snailfishNumber[i] == '[')
                    {
                        level++;
                    }
                    else if (snailfishNumber[i] == ']')
                    {
                        level--;
                    }
                    else if (level == 1 && snailfishNumber[i] == ',')
                    {
                        var left = snailfishNumber[1..i];
                        var right = snailfishNumber[(i+1)..(snailfishNumber.Length - 1)];
                        return 3 * Magnitude(left) + 2 * Magnitude(right);
                    }
                    i++;
                }
                throw new Exception("Misformed pair");
            }

            return int.Parse(snailfishNumber);
        }

        private string Add(string left, string right)
        {
            var s = $"[{left},{right}]";
            return Reduce(s);
        }

        private string Reduce(string snailfishNumber)
        {
            var current = snailfishNumber;
            var reduced = ReduceOnce(snailfishNumber);
            while (reduced != current)
            {
                current = reduced;
                reduced = ReduceOnce(current);
            }
            return reduced;
        }

        private string ReduceOnce(string snailfishNumber)
        {
            var exploded = Explode(snailfishNumber);
            if (exploded != snailfishNumber)
            {
                return exploded;
            }

            return Split(snailfishNumber);
        }

        private string Explode(string snailfishNumber)
        {
            var level = 0;
            var i = 0;
            var lastVIndex = -1;
            while (i < snailfishNumber.Length)
            {
                if (snailfishNumber[i] == '[')
                {
                    level++;
                }
                else if (snailfishNumber[i] == ']')
                {
                    level--;
                }
                else if (level == 5)
                {
                    // Explode this
                    var resultString = "";
                    var j = 0;
                    var v = "";
                    while (snailfishNumber[i + j] != ',')
                    {
                        v = v + snailfishNumber[i + j];
                        j++;
                    }

                    if (lastVIndex > -1)
                    {
                        var leftVal = int.Parse(v);
                        var k = 0;
                        var pv = "";
                        var kv = snailfishNumber[lastVIndex - k];
                        while (lastVIndex - k > 0 && kv != '[' && kv != ']' && kv != ',')
                        {
                            pv = kv + pv;
                            k++;
                            kv = snailfishNumber[lastVIndex - k];
                        }

                        var prevVal = int.Parse(pv);
                        var newVal = leftVal + prevVal;
                        resultString = snailfishNumber[..(lastVIndex - k + 1)] + newVal.ToString();
                        if (lastVIndex < i - 1)
                        {
                            resultString = resultString + snailfishNumber[(lastVIndex + 1)..(i - 1)];
                        }
                    }
                    else
                    {
                        resultString = snailfishNumber[..(i - 1)];
                    }

                    resultString = resultString + "0";

                    j++;
                    var w = "";
                    while (snailfishNumber[i + j] != ']')
                    {
                        w = w + snailfishNumber[i + j];
                        j++;
                    }
                    var ind = i + j;
                    var rightVal = int.Parse(w);

                    if (Regex.Match(snailfishNumber[(i+j)..], "\\d").Success)
                    {
                        while (snailfishNumber[i + j] == ']' || snailfishNumber[i + j] == '[' || snailfishNumber[i + j] == ',')
                        {
                            j++;
                        }
                        resultString = resultString + snailfishNumber[(ind + 1)..(i + j)];
                        var rv = "";
                        var l = 0;
                        while (snailfishNumber[i + j + l] != ']' && snailfishNumber[i + j + l] != '[' && snailfishNumber[i + j + l] != ',')
                        {
                            rv = rv + snailfishNumber[i + j + l];
                            l++;
                        }
                        var nextVal = int.Parse(rv);
                        var newRightVal = rightVal + nextVal;
                        resultString = resultString + newRightVal.ToString() + snailfishNumber[(i + j + l)..];
                    }
                    else
                    {
                        resultString = resultString + snailfishNumber[(ind + 1)..];
                    }

                    return resultString;
                }
                else if (snailfishNumber[i] != ',')
                {
                    lastVIndex = i;
                }
                i++;
            }

            return snailfishNumber;
        }

        private string Split(string snailfishNumber)
        {
            var changed = false;
            return Regex.Replace(snailfishNumber, "\\d{2,}", (match) =>
            {
                if (changed)
                {
                    return match.Value;
                }
                changed = true;
                var val = int.Parse(match.Value);
                return $"[{val / 2},{(val + 1) / 2}]";
            });
        }
    }
}

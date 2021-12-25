using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AoCBase;

namespace AoC2021.Solvers
{
    public class Solver24 : BaseSolver
    {
        // QQ Make them all big integers

        private List<Instruction> instructions;

        public Solver24()
        {
            instructions = InputReader<Instruction>().ReadInputAsLines().ToList();
        }

        public override string Solve_1()
        {
            // By looking at the code for my input, should be valid iff;
            // i_0 + 7 = i_13
            // i_1 - 3 = i_12
            // i_2 - 5 = i_11
            // i_3 - 8 = i_4
            // i_5 - 1 = i_10
            // i_6 - 6 = i_7
            // i_8 + 3 = i_9

            // So the answer should be: 29991993698469

            // (part 2) smallest should be: 14691271141118

            // Yes, I used a spreadsheet, split by sections for each digit, noticing that they're mostly the same with different numbers added
            // and some where, if you've chosen the numbers correctly, you can make z smaller. For my input, that led to the above.
            // I used the below to check my answers did actually turn out valid, and play around a bit, but the above (and the large amount of effort 
            // working it out) was how the solve happened.
            
            // I did try it programmatically first in a few different ways. But this is where I got to.

            // this was while (true) when using it, obviously, I've just toggled it so that running all for a combined runtime doesn't break;
            while (false)
            {
                var input = Console.ReadLine();
                if (input.Length != 14)
                {
                    Console.WriteLine("wrong length");
                }
                else
                {
                    TestInput(input);
                }
            }
            return "29991993698469";
        }

        public override string Solve_2()
        {
            return "14691271141118";
        }

        private void TestInput(string input)
        {
            var current = new State(0, 0, 0, 0);
            var i = 0;
            foreach (var instruction in instructions)
            {
                if (instruction.Type == InstructionType.Input)
                {
                    current = current.UpdateValue(instruction.A, int.Parse(input[i].ToString()));
                    i++;
                }
                else
                {
                    current = current.RunNonInputInstruction(instruction);
                }
            }
            Console.WriteLine(current.ToString());
        }

        private record class State(BigInteger W, BigInteger X, BigInteger Y, BigInteger Z)
        {
            public State UpdateValue(char register, BigInteger value)
            {
                return register switch
                {
                    'w' => new State(value, X, Y, Z),
                    'x' => new State(W, value, Y, Z),
                    'y' => new State(W, X, value, Z),
                    'z' => new State(W, X, Y, value),
                    _ => throw new Exception("Unknown register"),
                };
            }

            public BigInteger Value(char register)
            {
                return register switch
                {
                    'w' => W,
                    'x' => X,
                    'y' => Y,
                    'z' => Z,
                    _ => throw new Exception("Unknown register"),
                };
            }

            public State RunNonInputInstruction(Instruction instruction)
            {
                BigInteger b = instruction.Bval ?? Value(instruction.Bvar.Value);
                switch (instruction.Type)
                {
                    case InstructionType.Add:
                        return UpdateValue(instruction.A, Value(instruction.A) + b);
                    case InstructionType.Multiply:
                        return UpdateValue(instruction.A, Value(instruction.A) * b);
                    case InstructionType.Divide:
                        // C# integer division rounds towards 0 already
                        if (b == 0)
                        {
                            return null;
                        }
                        return UpdateValue(instruction.A, Value(instruction.A) / b);
                    case InstructionType.Mod:
                        if (Value(instruction.A) < 0 || b <= 0)
                        {
                            return null;
                        }
                        return UpdateValue(instruction.A, Value(instruction.A) % b);
                    case InstructionType.EqualityTest:
                        return UpdateValue(instruction.A, (Value(instruction.A) == b) ? 1 : 0);
                    default:
                        throw new Exception("Unexpected instruction type");
                }
            }
        }

        private class Instruction
        {
            public InstructionType Type;
            public char A;
            public char? Bvar;
            public int? Bval;

            public Instruction(InstructionType Type, char A, string B)
            {
                this.Type = Type;
                this.A = A;
                var isVal = int.TryParse(B, out var val);
                if (isVal)
                {
                    Bval = val;
                }
                else if (B.Length > 0)
                {
                    Bvar = B[0];
                }
            }

            public static Instruction Parse(string line)
            {
                var parts = line.Split(' ');
                return new Instruction(ParseType(parts[0]), parts[1][0], parts.Length > 2 ? parts[2] : "");
            }

            private static InstructionType ParseType(string type)
            {
                return type switch
                {
                    "inp" => InstructionType.Input,
                    "add" => InstructionType.Add,
                    "mul" => InstructionType.Multiply,
                    "div" => InstructionType.Divide,
                    "mod" => InstructionType.Mod,
                    "eql" => InstructionType.EqualityTest,
                    _ => throw new Exception("Unexpected instruction"),
                };
            }
        }

        public enum InstructionType
        {
            Input,
            Add,
            Multiply,
            Divide,
            Mod,
            EqualityTest
        }
    }
}

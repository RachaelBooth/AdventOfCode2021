using AoCBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoC2021.Solvers
{
    public class Solver02 : BaseSolver
    {
        private List<MovementInstruction> instructions;

        public Solver02()
        {
            instructions = InputReader<MovementInstruction>().ReadInputAsLines().ToList();
        }

        public override string Solve_1()
        {
            var position = Vector2.Zero;
            foreach (var instruction in instructions)
            {
                position = instruction.ApplyBasic(position);
            }
            return (position.X * position.Y).ToString();
        }

        public override string Solve_2()
        {
            var position = Vector3.Zero;
            foreach (var instruction in instructions)
            {
                position = instruction.ApplyAdvanced(position);
            }
            return ((int) position.X * (int) position.Y).ToString();
        }

        private class MovementInstruction
        {
            private long x;
            private MovementInstructionType type;
            
            internal enum MovementInstructionType
            {
                Forward,
                Down
            }

            public MovementInstruction(long x, MovementInstructionType type)
            {
                this.x = x;
                this.type = type;
            }

            public Vector2 ApplyBasic(Vector2 current)
            {
                return type switch
                {
                    MovementInstructionType.Forward => current + new Vector2(x, 0),
                    MovementInstructionType.Down => current + new Vector2(0, x),
                    _ => throw new NotImplementedException("Unexpected instruction type"),
                };
            }

            public Vector3 ApplyAdvanced(Vector3 current)
            {
                return type switch
                {
                    MovementInstructionType.Forward => current + new Vector3(x, current.Z * x, 0),
                    MovementInstructionType.Down => current + new Vector3(0, 0, x),
                    _ => throw new NotImplementedException("Unexpected instruction type"),
                };
            }

            public static MovementInstruction Parse(string line)
            {
                var parts = line.Split(' ');
                return parts[0] switch
                {
                    "forward" => new MovementInstruction(long.Parse(parts[1]), MovementInstructionType.Forward),
                    "up" => new MovementInstruction(-long.Parse(parts[1]), MovementInstructionType.Down),
                    "down" => new MovementInstruction(long.Parse(parts[1]), MovementInstructionType.Down),
                    _ => throw new NotImplementedException("Unexpected instruction direction"),
                };
            }
        }
    }
}

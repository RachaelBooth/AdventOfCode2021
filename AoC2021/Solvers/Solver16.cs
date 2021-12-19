using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AoCBase;

namespace AoC2021.Solvers
{
    public class Solver16 : BaseSolver
    {
        private string Transmission;
        private Packet TransmittedPacket;

        public Solver16()
        {
            Transmission = InputReader<string>().ReadInputAsLines().First();
            TransmittedPacket = ParsePackets();
        }

        public override string Solve_1()
        {
            var result = SumVersionNumbers(TransmittedPacket);
            return result.ToString();
        }

        public override string Solve_2()
        {
            var result = EvaluatePacket(TransmittedPacket);
            return result.ToString();
        }

        private long EvaluatePacket(Packet packet)
        {
            if (packet.Type == 4)
            {
                return packet.Value.Value;
            }

            if (packet.Type == 0)
            {
                return packet.SubPackets.Sum(p => EvaluatePacket(p));
            }

            if (packet.Type == 1)
            {
                return packet.SubPackets.Aggregate((long) 1, (current, next) => current * EvaluatePacket(next));
            }

            if (packet.Type == 2)
            {
                return packet.SubPackets.Min(p => EvaluatePacket(p));
            }

            if (packet.Type == 3)
            {
                return packet.SubPackets.Max(p => EvaluatePacket(p));
            }

            if (packet.Type == 5)
            {
                return EvaluatePacket(packet.SubPackets[0]) > EvaluatePacket(packet.SubPackets[1]) ? 1 : 0;
            }

            if (packet.Type == 6)
            {
                return EvaluatePacket(packet.SubPackets[0]) < EvaluatePacket(packet.SubPackets[1]) ? 1 : 0;
            }

            if (packet.Type == 7)
            {
                return EvaluatePacket(packet.SubPackets[0]) == EvaluatePacket(packet.SubPackets[1]) ? 1 : 0;
            }

            throw new Exception("Problem");
        }

        private int SumVersionNumbers(Packet packet)
        {
            return packet.Version.Value + packet.SubPackets.Sum(c => SumVersionNumbers(c));
        }

        private Packet ParsePackets()
        {
            var outerPacket = new Packet();
            var currentPacket = outerPacket;
            var section = new List<char>();
            foreach (var bit in Bits(Transmission))
            {
                if (!currentPacket.Version.HasValue)
                {
                    section.Add(bit);
                    currentPacket.IncrementLength();
                    if (section.Count == 3)
                    {
                        currentPacket.Version = Convert.ToInt32(string.Join("", section), 2);
                        section = new List<char>();
                    }
                }
                else if (!currentPacket.Type.HasValue)
                {
                    section.Add(bit);
                    currentPacket.IncrementLength();
                    if (section.Count == 3)
                    {
                        currentPacket.Type = Convert.ToInt32(string.Join("", section), 2);
                        section = new List<char>();
                    }
                }
                else if (currentPacket.Type == 4 && !currentPacket.Value.HasValue)
                {
                    section.Add(bit);
                    currentPacket.IncrementLength();
                    if (section.Count % 5 == 0 && section[^5] == '0')
                    {
                        var number = string.Join("", section.Where((c, i) => i % 5 != 0));
                        currentPacket.Value = Convert.ToInt64(number, 2);
                        section = new List<char>();
                        var parent = currentPacket.Parent;
                        while (parent.IsComplete())
                        {
                            currentPacket = parent;
                            parent = currentPacket.Parent;
                            if (parent == null)
                            {
                                return currentPacket;
                            }
                        }

                        var newChild = new Packet();
                        newChild.Parent = parent;
                        parent.SubPackets.Add(newChild);
                        currentPacket = newChild;
                    }
                }
                else if (currentPacket.Type != 4 && !currentPacket.LengthType.HasValue)
                {
                    currentPacket.IncrementLength();
                    currentPacket.LengthType = bit == '0' ? Packet.LengthTypeId.Zero : Packet.LengthTypeId.One;
                }
                else if (currentPacket.Type != 4 && !currentPacket.TotalLength.HasValue && !currentPacket.NumberOfSubPackets.HasValue)
                {
                    section.Add(bit);
                    currentPacket.IncrementLength();
                    if (currentPacket.LengthType == Packet.LengthTypeId.Zero && section.Count == 15)
                    {
                        currentPacket.TotalLength = Convert.ToInt32(string.Join("", section), 2);
                        var child = new Packet();
                        currentPacket.SubPackets.Add(child);
                        child.Parent = currentPacket;
                        currentPacket = child;
                        section = new List<char>();
                    }
                    else if (currentPacket.LengthType == Packet.LengthTypeId.One && section.Count == 11)
                    {
                        currentPacket.NumberOfSubPackets = Convert.ToInt32(string.Join("", section), 2);
                        var child = new Packet();
                        currentPacket.SubPackets.Add(child);
                        child.Parent = currentPacket;
                        currentPacket = child;
                        section = new List<char>();
                    }
                }
                else
                {
                    throw new Exception("Whoops");
                }
            }
            throw new Exception("Oh dear");
        }

        private IEnumerable<char> Bits(string hexString)
        {
            var i = 0;
            while (i < hexString.Length)
            {
                var bits = Convert.ToString(int.Parse(hexString[i].ToString(), System.Globalization.NumberStyles.HexNumber), 2).PadLeft(4, '0').ToCharArray();
                foreach (var bit in bits)
                {
                    yield return bit;
                }
                i++;
            }
        }

        private class Packet
        {
            public int? Type;
            public int? Version;
            public LengthTypeId? LengthType;
            public int? TotalLength;
            public int? NumberOfSubPackets;
            public long? Value;
            public List<Packet> SubPackets;
            public Packet Parent;
            public int ActualLength;

            public Packet()
            {
                SubPackets = new List<Packet>();
                ActualLength = 0;
            }

            public void IncrementLength()
            {
                ActualLength++;
                if (Parent != null)
                {
                    Parent.IncrementLength();
                }
            }

            public bool IsComplete()
            {
                if (!Type.HasValue)
                {
                    return false;
                }

                if (Type == 4)
                {
                    return Value.HasValue;
                }

                if (TotalLength.HasValue)
                {
                    return SubPackets.Sum(c => c.ActualLength) == TotalLength.Value;
                }

                if (NumberOfSubPackets.HasValue)
                {
                    return SubPackets.Count == NumberOfSubPackets.Value; ;
                }

                return false;
            }

            public enum PacketType
            {
                LiteralValue = 4,
                Unknown
            }

            public enum LengthTypeId
            {
                Zero,
                One
            }
        }
    }
}

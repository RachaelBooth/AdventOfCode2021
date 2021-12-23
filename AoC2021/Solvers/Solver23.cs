using AoCBase;
using AoCBase.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC2021.Solvers
{
    public class Solver23 : BaseSolver
    {
        private Dictionary<(int x, int y), char> initialMap;

        public Solver23()
        {
            var input = InputReader().Parse2DimensionalGrid();
            initialMap = new Dictionary<(int x, int y), char>(input.Where(kv => kv.Value == 'A' || kv.Value == 'B' || kv.Value == 'C' || kv.Value == 'D'));
        }

        public override string Solve_1()
        {
            /*var min = long.MaxValue;
            var start = new AmphipodBurrow(initialMap, 3);

            var current = start;
            while (true)
            {
                current.Draw();
                var action = Console.ReadLine();
                if (action == "stop")
                {
                    break;
                }

                if (action == "move")
                {
                    var from = Console.ReadLine().Split(',');
                    var to = Console.ReadLine().Split(',');
                    current = current.Move((int.Parse(from[0]), int.Parse(from[1])), (int.Parse(to[0]), int.Parse(to[1])));

                    if (current.IsComplete())
                    {
                        Console.WriteLine("Completed");
                        if (current.EnergyUsed < min)
                        {
                            min = current.EnergyUsed;
                        }
                        current = start;
                    }
                }
            }

            return min.ToString(); */
            var burrow = new AmphipodBurrow(initialMap, 3);
            var burrowState = burrow.State();
            var seenStates = new Dictionary<string, long>();
            seenStates.Add(burrowState.state, burrowState.energy);
            var edgeStates = new Dictionary<string, long>();
            edgeStates.Add(burrowState.state, burrowState.energy);

            // Found one example manually
            long minComplete = 16472;
            while (edgeStates.Any())
            {
                var newStates = new Dictionary<string, long>();
                foreach (var state in edgeStates)
                {
                    var b = AmphipodBurrow.FromState(state.Key, state.Value, 3);
                    foreach (var s in b.GetMoves())
                    {
                        if (s.IsComplete())
                        {
                            minComplete = Math.Min(s.EnergyUsed, minComplete);
                        }
                        else
                        {
                            var st = s.State();
                            if (seenStates.ContainsKey(st.state))
                            {
                                var old = seenStates[st.state];
                                var min = Math.Min(old, st.energy);
                                if (old > min)
                                {
                                    seenStates[st.state] = min;
                                    if (newStates.ContainsKey(st.state))
                                    {
                                        newStates[st.state] = min;
                                    }
                                    else
                                    {
                                        newStates.Add(st.state, min);
                                    }
                                }
                            }
                            else
                            {
                                if (st.energy < minComplete)
                                {
                                    newStates.Add(st.state, st.energy);
                                    seenStates.Add(st.state, st.energy);
                                }
                            }
                        }
                    }
                }
                edgeStates = newStates;
            }
            return minComplete.ToString();
        }

        public override string Solve_2()
        {
            var input = InputReader("2").Parse2DimensionalGrid();
            var init = new Dictionary<(int x, int y), char>(input.Where(kv => kv.Value == 'A' || kv.Value == 'B' || kv.Value == 'C' || kv.Value == 'D'));

            var burrow = new AmphipodBurrow(init, 5);
            var burrowState = burrow.State();
            var seenStates = new Dictionary<string, long>();
            seenStates.Add(burrowState.state, burrowState.energy);
            var edgeStates = new Dictionary<string, long>();
            edgeStates.Add(burrowState.state, burrowState.energy);

            long minComplete = long.MaxValue;
            while (edgeStates.Any())
            {
                var newStates = new Dictionary<string, long>();
                foreach (var state in edgeStates)
                {
                    var b = AmphipodBurrow.FromState(state.Key, state.Value, 5);
                    foreach (var s in b.GetMoves())
                    {
                        if (s.IsComplete())
                        {
                            minComplete = Math.Min(s.EnergyUsed, minComplete);
                        }
                        else
                        {
                            var st = s.State();
                            if (seenStates.ContainsKey(st.state))
                            {
                                var old = seenStates[st.state];
                                var min = Math.Min(old, st.energy);
                                if (old > min)
                                {
                                    seenStates[st.state] = min;
                                    if (newStates.ContainsKey(st.state))
                                    {
                                        newStates[st.state] = min;
                                    }
                                    else
                                    {
                                        newStates.Add(st.state, min);
                                    }
                                }
                            }
                            else
                            {
                                if (st.energy < minComplete)
                                {
                                    newStates.Add(st.state, st.energy);
                                    seenStates.Add(st.state, st.energy);
                                }
                            }
                        }
                    }
                }
                edgeStates = newStates;
            }
            return minComplete.ToString();
        }

        private class AmphipodBurrow
        {
            public long EnergyUsed;
            private Dictionary<(int x, int y), char> Map;
            private int MaxY;

            public AmphipodBurrow(Dictionary<(int x, int y), char> map, int maxY, long energyUsed = 0)
            {
                Map = map;
                EnergyUsed = energyUsed;
                MaxY = maxY;
            }

            public AmphipodBurrow CopyWithUpdate(char amphipodMoved, (int x, int y) oldLocation, (int x, int y) newLocation, long additionalEnergy)
            {
                var map = new Dictionary<(int x, int y), char>(Map);
                map.Remove(oldLocation);
                map.Add(newLocation, amphipodMoved);
                return new AmphipodBurrow(map, MaxY, EnergyUsed + additionalEnergy);
            }

            public bool IsComplete()
            {
                return Map.All(kv => kv.Key.x == RoomIndex(kv.Value));
            }

            public bool Matches(AmphipodBurrow other)
            {
                // Know these must be the same size, so can skip that check
                return Map.Keys.All(k => other.Map.ContainsKey(k) && Map[k] == other.Map[k]);
            }

            public AmphipodBurrow Move((int x, int y) oldLocation, (int x, int y) newLocation)
            {
                var map = new Dictionary<(int x, int y), char>(Map);
                var amphipod = map[(oldLocation)];
                map.Remove(oldLocation);
                map.Add(newLocation, amphipod);
                var diff = Math.Abs(oldLocation.x - newLocation.x) + Math.Abs(oldLocation.y - newLocation.y);
                var energy = EnergyMultiplier(amphipod) * diff;
                return new AmphipodBurrow(map, MaxY, EnergyUsed + energy);
            }

            public (string state, long energy) State()
            {
                var s = new StringBuilder();
                var y = 1;
                while (y <= MaxY)
                {
                    var x = 1;
                    while (x <= 11)
                    {
                        if (Map.ContainsKey((x, y)))
                        {
                            s.Append(Map[(x, y)]);
                        }
                        else
                        {
                            s.Append('.');
                        }
                        x++;
                    }
                    y++;
                }
                return (s.ToString(), EnergyUsed);
            }

            public static AmphipodBurrow FromState(string state, long energy, int maxY)
            {
                var map = new Dictionary<(int x, int y), char>();
                var y = 1;
                while (y <= maxY)
                {
                    var x = 1;
                    while (x <= 11)
                    {
                        var c = state[11 * (y - 1) + x - 1];
                        if (c != '.')
                        {
                            map.Add((x, y), c);
                        }
                        x++;
                    }
                    y++;
                }
                return new AmphipodBurrow(map, maxY, energy);
            }

            public void Draw()
            {
                Console.WriteLine("#############");
                var y = 1;
                while (y <= MaxY)
                {
                    var x = 0;
                    while (x <= 12)
                    {
                        if (y > 2 && (x < 2 || x > 10))
                        {
                            Console.Write(' ');
                        }
                        else if (y >= 2 && x % 2 == 0)
                        {
                            Console.Write('#');
                        }
                        else if (y == 2 && (x == 1 || x == 11))
                        {
                            Console.Write('#');
                        }
                        else if (y == 1 && (x == 0 || x == 12))
                        {
                            Console.Write('#');
                        }
                        else if (Map.ContainsKey((x, y)))
                        {
                            Console.Write(Map[(x, y)]);
                        }
                        else
                        {
                            Console.Write('.');
                        }
                        x++;
                    }
                    Console.WriteLine();
                    y++;
                }
                Console.WriteLine();
                Console.WriteLine($"Energy Used: {EnergyUsed}");
                Console.WriteLine();
            }

            public List<AmphipodBurrow> GetMoves()
            {
                var newStates = new List<AmphipodBurrow>();
                foreach (var amphipod in Map)
                {
                    var location = amphipod.Key;
                    var amphipodType = amphipod.Value;

                    if (CanMoveToDestination(location, amphipodType))
                    {
                        var destinationIndex = RoomIndex(amphipodType);
                        var steps = Math.Abs(destinationIndex - location.x) + location.y - 1;
                        var y = MaxY;
                        while (Map.ContainsKey((destinationIndex, y)))
                        {
                            y--;
                        }
                        if (y < 2)
                        {
                            throw new Exception("Room too full");
                        }
                        steps = steps + y - 1;
                        var destination = (destinationIndex, y);
                        var energy = steps * EnergyMultiplier(amphipodType);
                        // No point in bothering with any other possible moves ?
                        // return new List<AmphipodBurrow> { CopyWithUpdate(amphipodType, location, destination, energy) };
                        newStates.Add(CopyWithUpdate(amphipodType, location, destination, energy));
                    }

                    if (IsInDestinationRoomAndNotBlocking(location, amphipodType))
                    {
                        // No point in moving
                    }
                    else if (location.y > 1)
                    {
                        // At lower point in room
                        // (all hallway moves are moves to destination, so will be caught above)
                        if (IsBlockedInRoom(location))
                        {
                            // Can't move, blocked by other amphipod
                        }
                        else
                        {
                            // Can move if there's somewhere in hallway to stop, or if can move to destination room
                            // Don't need to consider destination room directly - if moving between rooms can always stop in hallway
                            // between them and then move from hall to room for same energy cost
                            var destinations = AvailableHallwayDestinations(location.x);
                            foreach (var destination in destinations)
                            {
                                var steps = Math.Abs(destination - location.x) + location.y - 1;
                                var energy = steps * EnergyMultiplier(amphipodType);
                                newStates.Add(CopyWithUpdate(amphipodType, location, (destination, 1), energy));
                            }
                        }
                    }
                }
                return newStates;
            }

            private bool IsBlockedInRoom((int x, int y) currentLocation)
            {
                if (currentLocation.y == 1)
                {
                    return false;
                }

                var y = currentLocation.y - 1;
                while (y > 1)
                {
                    if (Map.ContainsKey((currentLocation.x, y)))
                    {
                        return true;
                    }
                    y--;
                }
                return false;
            }

            private bool CanMoveToDestination((int x, int y) currentLocation, char amphipod)
            {
                if (currentLocation.x == RoomIndex(amphipod))
                {
                    return false;
                }

                if (!DestinationRoomIsReady(amphipod))
                {
                    return false;
                }

                if (IsBlockedInRoom(currentLocation))
                {
                    return false;
                }

                return HallwayIsUnblocked(currentLocation.x, RoomIndex(amphipod));
            }

            private List<int> AvailableHallwayDestinations(int fromX)
            {
                var destinations = new List<int>();
                var x = fromX - 1;
                while (x >= 1 && !Map.ContainsKey((x, 1)))
                {
                    if (x != 3 && x != 5 && x != 7 && x!= 9)
                    {
                        destinations.Add(x);
                    }
                    x--;
                }
                x = fromX + 1;
                while (x <= 11 && !Map.ContainsKey((x, 1)))
                {
                    if (x != 3 && x != 5 && x != 7 && x != 9)
                    {
                        destinations.Add(x);
                    }
                    x++;
                }
                return destinations;
            }


            private bool HallwayIsUnblocked(int startIndex, int endIndex)
            {
                var start = startIndex < endIndex ? startIndex + 1 : endIndex;
                var end = startIndex < endIndex ? endIndex : startIndex - 1;
                var x = start;
                while (x <= end)
                {
                    if (Map.ContainsKey((x, 1)))
                    {
                        return false;
                    }
                    x++;
                }
                return true;
            }

            private bool DestinationRoomIsReady(char amphipod)
            {
                var x = RoomIndex(amphipod);
                var i = 2;
                while (i <= MaxY)
                {
                    if (Map.ContainsKey((x, i)) && Map[(x, i)] != amphipod)
                    {
                        return false;
                    }
                    i++;
                }
                return true;
            }

            private bool IsInDestinationRoomAndNotBlocking((int x, int y) location, char amphipod)
            {
                if (location.y == 1)
                {
                    return false;
                }

                if (location.x != RoomIndex(amphipod))
                {
                    return false;
                }

                var i = location.y;
                while (i <= MaxY)
                {
                    if (Map.ContainsKey((location.x, i)) && Map[(location.x, i)] != amphipod)
                    {
                        // Blocking another amphipod
                        return false;
                    }
                    i++;
                }
                return true;
            }

            private static int RoomIndex(char amphipod)
            {
                return char.ToUpper(amphipod) switch
                {
                    'A' => 3,
                    'B' => 5,
                    'C' => 7,
                    'D' => 9,
                    _ => throw new Exception("Unknown amphipod"),
                };
            }

            private static long EnergyMultiplier(char amphipod)
            {
                return char.ToUpper(amphipod) switch
                {
                    'A' => 1,
                    'B' => 10,
                    'C' => 100,
                    'D' => 1000,
                    _ => throw new Exception("Unknown amphipod"),
                };
            }
        }
    }
}
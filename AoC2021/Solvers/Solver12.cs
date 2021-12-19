using System.Collections.Generic;
using System.Linq;
using AoCBase;

namespace AoC2021.Solvers
{
    public class Solver12 : BaseSolver
    {
        private Dictionary<string, HashSet<string>> paths;

        public Solver12()
        {
            paths = new Dictionary<string,HashSet<string>>();
            foreach (var path in InputReader<Path>().ReadInputAsLines())
            {
                paths.AddOption(path.Start, path.End);
                paths.AddOption(path.End, path.Start);
            }
        }

        public override string Solve_1()
        {
            var walks = new List<List<string>> { new List<string> { "start" } };
            var completedWalks = new List<List<string>>();
            while (walks.Any())
            {
                var nextWalks = new List<List<string>>();
                foreach (var walk in walks)
                {
                    foreach (var next in paths[walk[^1]])
                    {
                        if (next == "end")
                        {
                            var path = walk.ToList();
                            path.Add(next);
                            completedWalks.Add(path);
                        }
                        else if (char.IsUpper(next[0]) || !walk.Contains(next))
                        {
                            var path = walk.ToList();
                            path.Add(next);
                            nextWalks.Add(path);
                        }
                    }
                }
                walks = nextWalks;
            }
            return completedWalks.Count.ToString();
        }

        public override string Solve_2()
        {
             var walks = new List<(List<string> path, bool hasDuplicateSmall)> { (new List<string> { "start" }, false) };
             var completedWalks = new List<List<string>>();
             while (walks.Any())
             {
                 var nextWalks = new List<(List<string> path, bool hasDuplicateSmall)>();
                 foreach (var walk in walks)
                 {
                     foreach (var next in paths[walk.path[^1]])
                     {
                         if (next == "end")
                         {
                             var path = walk.path.ToList();
                             path.Add(next);
                             completedWalks.Add(path);
                         }
                         else if (char.IsUpper(next[0]) || !walk.path.Contains(next))
                         {
                             var path = walk.path.ToList();
                             path.Add(next);
                             nextWalks.Add((path, walk.hasDuplicateSmall));
                         }
                         else if (!walk.hasDuplicateSmall && next != "start")
                         {
                             var path = walk.path.ToList();
                             path.Add(next);
                             nextWalks.Add((path, true));
                         }
                     }
                 }
                 walks = nextWalks;
             }
             return completedWalks.Count.ToString();
        }

        private record class Path(string Start, string End)
        {
            public static Path Parse(string line)
            {
                var parts = line.Split('-');
                return new Path(parts[0], parts[1]);
            }
        }
    }
}

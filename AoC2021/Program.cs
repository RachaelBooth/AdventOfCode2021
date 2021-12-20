using System.Linq;
using System.Reflection;
using AoCHelper;

namespace AoC2021
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = Assembly.GetEntryAssembly().GetTypes().Where(type =>
                typeof(BaseProblem).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
            Solver.SolveLast();
        }
    }
}

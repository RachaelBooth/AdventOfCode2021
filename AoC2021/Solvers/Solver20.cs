using AoCBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021.Solvers
{
    public class Solver20 : BaseSolver
    {
        private string ImageEnhancement;
        private GridMap<bool> Image;

        public Solver20()
        {
            var input = InputReader<string, string>().ReadAsMultipartLines();
            ImageEnhancement = input.Item1[0];
            var dict = new Dictionary<(int x, int y), bool>();
            var y = 0;
            while (y < input.Item2.Count)
            {
                var x = 0;
                while (x < input.Item2[y].Length)
                {
                    if (input.Item2[y][x] == '#')
                    {
                        dict.Add((x, y), true);
                    }
                    x++;
                }
                y++;
            }
            Image = new GridMap<bool>(dict, false);
        }

        public override string Solve_1()
        {
            var output = Enhance(Image, 2);
            var result = output.FindAll(true).Count;
            return result.ToString();
        }

        public override string Solve_2()
        {
            var output = Enhance(Image, 50);
            var result = output.FindAll(true).Count;
            return result.ToString();
        }

        // Should probably have just used a HashSet and tracked which the default was, I'm not gaining anything from the GridMap.
        private GridMap<bool> Enhance(GridMap<bool> currentImage, int times)
        {
            var t = 0;
            var current = currentImage;
            while (t < times)
            {
                current = Enhance(current);
                t++;
            }
            return current;
        }

        private GridMap<bool> Enhance(GridMap<bool> currentImage)
        {
            // In my input, .../.../... => # and ###/###/### => .
            // So we swap the default, and we only consider points in range of something non-default

            var enhanced = new GridMap<bool>(new Dictionary<(int x, int y), bool>(), !currentImage.FixedDefault);
            var keys = currentImage.Keys();
            var minX = keys.Min(k => k.x) - 1;
            var maxX = keys.Max(k => k.x) + 1;
            var minY = keys.Min(k => k.y) - 1;
            var maxY = keys.Max(k => k.y) + 1;

            var x = minX;
            while (x <= maxX)
            {
                var y = minY;
                while (y <= maxY)
                {
                    var v = GetNextValue(
                        currentImage.ReadWithDefault((x - 1, y - 1)),
                        currentImage.ReadWithDefault((x, y - 1)),
                        currentImage.ReadWithDefault((x + 1, y - 1)),
                        currentImage.ReadWithDefault((x - 1, y)),
                        currentImage.ReadWithDefault((x, y)),
                        currentImage.ReadWithDefault((x + 1, y)),
                        currentImage.ReadWithDefault((x - 1, y + 1)),
                        currentImage.ReadWithDefault((x, y + 1)),
                        currentImage.ReadWithDefault((x + 1, y + 1))
                        );
                    if (v != enhanced.FixedDefault)
                    {
                        enhanced.Set((x, y), v);
                    }
                    y++;
                }
                x++;
            }
            return enhanced;
        }

        private Dictionary<(bool, bool, bool, bool, bool, bool, bool, bool, bool), bool> enchancementCache = new Dictionary<(bool, bool, bool, bool, bool, bool, bool, bool, bool), bool>();
        
        private bool GetNextValue(bool a, bool b, bool c, bool d, bool e, bool f, bool g, bool h, bool i)
        {
            var k = (a, b, c, d, e, f, g, h, i);
            if (enchancementCache.ContainsKey(k))
            {
                return enchancementCache[k];
            }

            var s = $"{(a ? 1 : 0)}{(b ? 1 : 0)}{(c ? 1 : 0)}{(d ? 1 : 0)}{(e ? 1 : 0)}{(f ? 1 : 0)}{(g ? 1 : 0)}{(h ? 1 : 0)}{(i ? 1 : 0)}";
            var n = Convert.ToInt32(s, 2);
            var r = ImageEnhancement[n] == '#';
            enchancementCache.Add(k, r);
            return r;
        }
    }
}

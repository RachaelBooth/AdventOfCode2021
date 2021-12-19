using AoCBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021.Solvers
{
    public class Solver19 : BaseSolver
    {
        private Dictionary<int, ScannerData> scannerData;
        private HashSet<Vector3Int> beacons;
        private List<(int scanner, Vector3Int location, int directionIndex)> scanners;


        public Solver19()
        {
            scannerData = InputReader<ScannerData>().ReadInputAsLineGroups().ToDictionary(s => s.ScannerNumber);
            scanners = new List<(int scanner, Vector3Int location, int directionIndex)>
            {
                // Take scanner 0 to be the origin
                (0, new Vector3Int(0, 0, 0), 0)
            };
            beacons = new HashSet<Vector3Int>();
            foreach (var beacon in scannerData[0].Beacons)
            {
                beacons.Add(beacon[0]);
            }
            ProduceMap(12);
        }

        public override string Solve_1()
        {
            return beacons.Count.ToString();
        }

        public override string Solve_2()
        {
            var maxDist = 0;
            var i = 0;
            while (i < scanners.Count)
            {
                var j = i + 1;
                while (j < scanners.Count)
                {
                    var distance = scanners[i].location - scanners[j].location;
                    var md = Math.Abs(distance.X) + Math.Abs(distance.Y) + Math.Abs(distance.Z);
                    if (md > maxDist)
                    {
                        maxDist = md;
                    }
                    j++;
                }
                i++;
            }
            return maxDist.ToString();
        }

        private void ProduceMap(int minOverlap)
        {
            var last = 1;
            while (scanners.Count < scannerData.Count)
            {
                var scannersToTry = scannerData.Keys.Where(k => !scanners.Any(s => s.scanner == k));
                foreach (var scanner in scannersToTry)
                {
                    TryAddScannerToMap(scannerData[scanner], minOverlap);
                }
                if (scanners.Count == last)
                {
                    throw new Exception("couldn't add any scanners");
                }
                last = scanners.Count;
            }
        }

        private void TryAddScannerToMap(ScannerData scanner, int minOverlap)
        {
            foreach (var mappedScanner in scanners)
            {
                var success = TryAddScannerOverlappingWithMappedScanner(scanner, mappedScanner, minOverlap);
                if (success)
                {
                    return;
                }
            }
        }

        private bool TryAddScannerOverlappingWithMappedScanner(ScannerData scanner, (int scanner, Vector3Int location, int directionIndex) mappedScanner, int minOverlap)
        {
            if (scanner.CheckedOtherScanners.Contains(mappedScanner.scanner))
            {
                return false;
            }

            var potentialOverlaps = scannerData[mappedScanner.scanner].Distances.Aggregate(0, (current, next) =>
            {
                if (!scanner.Distances.ContainsKey(next.Key))
                {
                    return current;
                }

                return current + Math.Min(next.Value.Count, scanner.Distances[next.Key].Count);
            });
            if (potentialOverlaps < minOverlap)
            {
                scanner.CheckedOtherScanners.Add(mappedScanner.scanner);
                return false;
            }

            var overlappingDistances = scannerData[mappedScanner.scanner].Distances.Where(kv => scanner.Distances.ContainsKey(kv.Key)).ToList();
            foreach (var distance in overlappingDistances)
            {
                foreach (var mappedOption in distance.Value)
                {
                    var beaconA = scannerData[mappedScanner.scanner].Beacons[mappedOption.index1][mappedScanner.directionIndex] + mappedScanner.location;
                    var beaconB = scannerData[mappedScanner.scanner].Beacons[mappedOption.index2][mappedScanner.directionIndex] + mappedScanner.location;
                    foreach (var scannerOption in scanner.Distances[distance.Key])
                    {
                        var orientation = 0;
                        while (orientation < 24)
                        {
                            var displacementA = beaconA - scanner.Beacons[scannerOption.index1][orientation];
                            if (scanner.Beacons[scannerOption.index2][orientation] + displacementA == beaconB)
                            {
                                // this orientation is possible
                                var scannerBeacons = scanner.Beacons.Select(b => b[orientation] + displacementA);
                                var mappedScannerBeacons = scannerData[mappedScanner.scanner].Beacons.Select(b => b[mappedScanner.directionIndex] + mappedScanner.location);
                                var overlap = scannerBeacons.Intersect(mappedScannerBeacons);
                                // for now we assume there are no fakes - i.e. nothing where this is wrong
                                // (e.g. shows 12 overlap but also one that's present in one but not the other)
                                if (overlap.Count() >= minOverlap)
                                {
                                    // Success!
                                    scanners.Add((scanner.ScannerNumber, displacementA, orientation));
                                    foreach (var b in scannerBeacons)
                                    {
                                        beacons.Add(b);
                                    }
                                    return true;
                                }
                            }

                            var displacementB = beaconB - scanner.Beacons[scannerOption.index1][orientation];
                            if (scanner.Beacons[scannerOption.index2][orientation] + displacementB == beaconA)
                            {
                                // this orientation is possible
                                var scannerBeacons = scanner.Beacons.Select(b => b[orientation] + displacementB);
                                var mappedScannerBeacons = scannerData[mappedScanner.scanner].Beacons.Select(b => b[mappedScanner.directionIndex] + mappedScanner.location);
                                var overlap = scannerBeacons.Intersect(mappedScannerBeacons);
                                // for now we assume there are no fakes - i.e. nothing where this is wrong
                                // (e.g. shows 12 overlap but also one that's present in one but not the other)
                                if (overlap.Count() >= minOverlap)
                                {
                                    // Success!
                                    scanners.Add((scanner.ScannerNumber, displacementB, orientation));
                                    foreach (var b in scannerBeacons)
                                    {
                                        beacons.Add(b);
                                    }
                                    return true;
                                }
                            }

                            orientation++;
                        }
                    }
                }
            }
            scanner.CheckedOtherScanners.Add(mappedScanner.scanner);
            return false;
        }

        private class ScannerData
        {
            public int ScannerNumber;
            public List<List<Vector3Int>> Beacons;
            public Dictionary<double, List<(int index1, int index2)>> Distances;
            public List<int> CheckedOtherScanners;

            public ScannerData(int ScannerNumber, IEnumerable<Vector3Int> Beacons)
            {
                this.ScannerNumber = ScannerNumber;
                this.Beacons = Beacons.Select(b => Vector3Int.GetAllOrientations(b)).ToList();
                Distances = new Dictionary<double, List<(int index1, int index2)>>();
                var i = 0;
                while (i < this.Beacons.Count)
                {
                    var j = i + 1;
                    while (j < this.Beacons.Count)
                    {
                        var diff = this.Beacons[i][0] - this.Beacons[j][0];
                        var distance = diff.Magnitude();
                        Distances.AddOptions<double, (int index1, int index2)>(distance, (i, j));
                        j++;
                    }
                    i++;
                }
                CheckedOtherScanners = new List<int>();
            }

            public static ScannerData Parse(List<string> lines)
            {
                var number = int.Parse(lines[0].Split(' ')[2]);
                var beacons = lines.Skip(1).Select(l =>
                {
                    var parts = l.Split(',');
                    return new Vector3Int(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
                });
                return new ScannerData(number, beacons);
            }
        }
    }
}

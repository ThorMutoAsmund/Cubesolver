using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubesolver
{
    class Program
    {

        static void Main(string[] args)
        {
            //Demo();
            //SpeedTest();
            PathFinder();
        }


        static void PathFinder()
        {
            var pf = new NPathFinder();

            // 1 = 28 states
            // 2 = 591 states
            // 3 = 11738 states
            // 4 = 207507 states
            // 5 = 3683530 states (3.0 seconds, 82 MB)   (without state storage 19 MB)
            // 6 = 65304456 states (57 (35 with opt.) seconds, 1920 MB) (without state storage 71 MB)
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            pf.GenerateBaseSet(6);
            stopWatch.Stop();
            Console.WriteLine($"Exceution time: {stopWatch.Elapsed} ms");
            //pf.GenerateFile();
            Console.WriteLine($"Done! Size={pf.KeySetSize}");

            Console.ReadKey();
        }

        static void SpeedTest()
        { 
            var cube = NCube.Id;

            Stopwatch stopWatch = new Stopwatch();
            int cnt = 200000000;
            stopWatch.Start();
            while (cnt > 0)
            {
                //r = rand.Next(12);  
                // iB 12.6 sek
                cube.Turn(NCube.iB);
                cnt--;
            }
            stopWatch.Stop();

            cube.Display();
            Console.WriteLine($"Exceution time: {stopWatch.Elapsed} ms");

            Console.ReadKey();
        }

        static void Demo()
        { 
            var cube = NCube.Id;

            ConsoleKeyInfo keyInfo;
            do
            {
                Console.Clear();
                cube.Display();
                keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.U:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ? NCube.dU : NCube.iU) : NCube.tU);
                        break;
                    case ConsoleKey.D:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ? NCube.dD : NCube.iD) : NCube.tD);
                        break;
                    case ConsoleKey.F:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ? NCube.dF : NCube.iF) : NCube.tF);
                        break;
                    case ConsoleKey.B:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ? NCube.dB : NCube.iB) : NCube.tB);
                        break;
                    case ConsoleKey.R:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ? NCube.dR : NCube.iR) : NCube.tR);
                        break;
                    case ConsoleKey.L:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ? NCube.dL : NCube.iL) : NCube.tL);
                        break;
                }

            }
            while (keyInfo.Key != ConsoleKey.Escape && keyInfo.Key != ConsoleKey.Q);
        }
    }
}

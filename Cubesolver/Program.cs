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

            // It took 1:40 to figure out there are 77324656 possible states reachable with depth 6
            pf.GenerateBaseSet(3);
            //pf.GenerateFile();
            Console.WriteLine($"Done! Size={pf.KeyStoreSize}");

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

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
            var cube = NCube.Id;

            Stopwatch stopWatch = new Stopwatch();
            int cnt = 100000000;
            stopWatch.Start();
            var rand = new Random();
            int r;
            while (cnt > 0)
            {
                //r = rand.Next(12);  
                // iB 13.4 sek
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
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? NCube.iU : NCube.tU);
                        break;
                    case ConsoleKey.D:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? NCube.iD : NCube.tD);
                        break;
                    case ConsoleKey.F:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? NCube.iF : NCube.tF);
                        break;
                    case ConsoleKey.B:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? NCube.iB : NCube.tB);
                        break;
                    case ConsoleKey.R:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? NCube.iR : NCube.tR);
                        break;
                    case ConsoleKey.L:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? NCube.iL : NCube.tL);
                        break;
                }

            }
            while (keyInfo.Key != ConsoleKey.Escape && keyInfo.Key != ConsoleKey.Q);
        }
    }
}

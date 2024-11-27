using System;
using System.Diagnostics;
using static Cubesolver.NCube;

namespace Cubesolver
{
    public class NPathFinderProgram
    {
        public void Run()
        {
            //Demo();
            //SpeedTest();
            FindPaths();
            //Console.WriteLine(NPathFinder.TriggerSetToString(68));
            //Console.WriteLine(NPathFinder.TriggerSetToInverseString(68));
        }

        /*
         * 
            Mine værste cases pt:
            Pi: 20, 26, 51, 58
            H: 29, 30
            AS: 3, 6, *29*, *32*, 41, *50*, 53, 54
         * 
         * */


        static void FindPaths()
        {
            var pf = new NPathFinder(storeOnlyFirstMatchingTriggerSet: true);


            //pf.SetCaseFromInverse(new int[] { tF, iU, iR, tU, tR, tD, iR, tU, tR, tU, iD, iF, iR, tU, tR }); // ZBLL L 60
            //pf.SetCaseFromInverse(new int[] { tF, tU, tR, dU, iR, tU, tR, tU, iR, iF, tR, dU, iR, iU, tR, iU, iR }); // ZBLL Pi 19

            //pf.SetCaseFromInverse(new int[] { iR, iU, tR, iU, iR, tU, tR, tU, iD, dR, tU, iR, tU, tR, iU, tR, iU, dR, tD }); // ZBLL AS 29
            // Solution found 11/4/2022:  R U' R' F D U R U' R' U R U' R' D' R' F' R

            //pf.SetCaseFromInverse(new int[] { iR, iU, tR, iU, iR, iU, tR, tU, iR, iU, iR, iD, tR, tU, iR, tD, tR, dU, tR }); // ZBLL AS 32

            //pf.SetCaseFromInverse(new int[] { dU, iR, tF, iU, iF, iU, tR, tF, iU, iR, iU, tR, iF }); // ZBLL AS 6

            //pf.SetCaseFromInverse(new int[] { tR, dU, iR, iU, tR, tU, iL, tU, tL, dU, iR, iU, iL, dU, tL }); // ZBLL AS 54

            //pf.SetCaseFromInverse(new int[] { tR, tF, iU, dR, iU, tR, iU, iR, dU, dR, tU, iF, iR }); // ZBLL PI 58

            //pf.SetCaseFromInverse(new int[] { iB, dR, tU, iR, tU, iR, tU, tR, dU, tR, iU, dR, tB }); // ZBLL PI 51

            //pf.SetCaseFromInverse(new int[] { iR, iU, tR, tU, dR, tU, tL, iU, dR, tU, iL, tU, iR, dU, tR }); // ZBLL AS 3

            //pf.SetCaseFromInverse(new int[] { tR, tU, iR, tU, tR, dU, dR, dU, tL, iU, tR, tU, iL, tU, iR, tU, tR }); // ZBLL H 29
            // tR tU iR tU tR dU dR dU tL iU tR tU iL tU iR tU tR


            //pf.SetCaseFromInverse(new int[] { _iR, _iU, _R, _iU, _iR, _U2, _R2, _U2, _iL, _U, _iR, _iU, _L, _iU, _R, _iU, _iR }); // ZBLL H 30
            // iR iU tR iU iR dU dR dU iL tU iR iU tL iU tR iU iR 

            pf.GenerateBaseSet(2);

            pf.PrintBaseSet();

            pf.SetCaseFromInverse(new int[] { _S }); 


            // Pi: 20, 26
            // H: 29, 30
            // AS: 3, 6


            pf.FindPathsToCase(2);
            //Console.WriteLine($"Done! Size={pf.KeySetSize}");
        }

        static void PathFinderSpeedTest()
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
                cube.Turn(NCube._iB);
                cnt--;
            }
            stopWatch.Stop();

            Visualizer.Display(cube.C, cube.E);
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
                Visualizer.Display(cube.C, cube.E);
                keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.U:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ? NCube._U2 : NCube._iU) : NCube._U);
                        break;
                    case ConsoleKey.D:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ? NCube._D2 : NCube._iD) : NCube._D);
                        break;
                    case ConsoleKey.F:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ? NCube._F2 : NCube._iF) : NCube._F);
                        break;
                    case ConsoleKey.B:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ? NCube._B2 : NCube._iB) : NCube._B);
                        break;
                    case ConsoleKey.R:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ? NCube._R2 : NCube._iR) : NCube._R);
                        break;
                    case ConsoleKey.L:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control) ? NCube._L2 : NCube._iL) : NCube._L);
                        break;
                    case ConsoleKey.E:
                        cube.Turn(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? NCube._iwR : NCube._wR);
                        break;
                }

            }
            while (keyInfo.Key != ConsoleKey.Escape && keyInfo.Key != ConsoleKey.Q);
        }
    }
}

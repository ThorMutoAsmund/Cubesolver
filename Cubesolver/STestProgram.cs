using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubesolver
{
    using static SCube;
    public class STestProgram
    {
        private const int GodsNumber = 20;
        private const int ForwardDepth = 6;
        private const int ReverseDepth = GodsNumber - ForwardDepth;


        private HashSet<SCube> forwardBuffer = new HashSet<SCube>(80000000);
        private Queue<SCube> forwardQueue = new Queue<SCube>();
        private HashSet<SCube> reverseBuffer = new HashSet<SCube>();
        private HashSet<(SCube, SCube)> solutions = new HashSet<(SCube, SCube)>();
        public void Run()
        {
            var initial = SCube.Id;

            forwardBuffer.Add(initial);
            forwardQueue.Enqueue(initial);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (ForwardDepth > 0)
            {
                Forward();
            }
            stopWatch.Stop();
            Console.WriteLine($"Forward execution time: {stopWatch.ElapsedMilliseconds} ms. States={forwardBuffer.Count}");

            var scramble = SCube.Id;
            //scramble.Turn(Visualizer.FromString("R' U' F B2 L2 D2 R2 U R2 U2 B2 D' R2 F D' L2 D' L2 R2 U' L F R' U' R' U' F"));
            scramble.Turn(Visualizer.FromString("R' U' F"));

            return;

            //stopWatch = new Stopwatch();
            //stopWatch.Start();
            
            //if (forwardBuffer.Contains(scramble))
            //{
            //    solutions.Add((scramble, scramble));
            //}
            //else
            //{
            //    Search(scramble, 1);
            //}

            //stopWatch.Stop();
            //Console.WriteLine($"Reverse execution time: {stopWatch.ElapsedMilliseconds} ms. Solutions={solutions.Count}");
        }


        private void Forward()
        {
            int[] lastIndex = new int[ForwardDepth+2];
            lastIndex[ForwardDepth] = int.MaxValue;
            int depth = 1;
            int idx = 0;
            lastIndex[depth] = forwardBuffer.Count - 1;

            while (forwardQueue.Count > 0)
            {
                var c = forwardQueue.Dequeue();

                for (int i = 0; i < AllTurns.Length; i++)
                {
                    var t = AllTurns[i];
                    c.Turn(t);
                    if (!forwardBuffer.Contains(c))
                    {
                        forwardBuffer.Add(c);
                        if (depth < ForwardDepth)
                        {
                            forwardQueue.Enqueue(c);
                        }
                    }
                    c.Turn(t ^ 1);
                }
                if (idx == lastIndex[depth])
                {
                    depth++;
                    lastIndex[depth] = forwardBuffer.Count - 1;
                }
                idx++;
            }
        }

        private void ForwardOld(SCube c, int depth, int lastPos = -1)
        {
            for (int i = 0; i < AllTurns.Length; i++)
            {
                if (i / 3 == lastPos)
                {
                    continue;
                }
                var t = AllTurns[i];
                c.Turn(t);
                if (!forwardBuffer.Contains(c))
                {
                    forwardBuffer.Add(c);
                    if (depth < ForwardDepth)
                    {
                        ForwardOld(c, depth + 1, i / 3);
                    }
                }
                c.Turn(t ^ 1);
            }
        }
        private void Search(SCube c, int depth, int lastPos = -1)
        {
            for (int i = 0; i < AllTurns.Length; i++)
            {
                if (i / 3 == lastPos)
                {
                    continue;
                }
                var t = AllTurns[i];
                c.Turn(t);
                if (!reverseBuffer.Contains(c))
                {
                    reverseBuffer.Add(c);
                    if (forwardBuffer.Contains(c))
                    {
                        solutions.Add((c, c));
                    }
                    else
                    {
                        if (depth < ReverseDepth)
                        {
                            Search(c, depth + 1, i / 3);
                        }
                    }
                }
                c.Turn(t ^ 1);
            }
        }
    }
}

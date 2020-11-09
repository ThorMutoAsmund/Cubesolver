using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cubesolver.Cube;
namespace Cubesolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Cube a = Id;

            //(byte, byte, byte, byte, byte, byte, byte, byte, UInt32, UInt32, UInt32, UInt32, UInt32)
            //    x3 = (0, 4, 8, 12, 4, 4, 4, 12, 0xfU << 0, 0xfU << 4, 0xfU << 8, 0xfU << 12, ~(0xfU << 0 | 0xfU << 4 | 0xfU << 8 | 0xfU << 12));

            //(byte, byte, byte, byte, byte, byte, byte, byte, UInt64, UInt64, UInt64, UInt64, UInt64)
            //    x4 = (0, 4, 8, 12, 4, 4, 4, 12, 0xfU << 0, 0xfU << 4, 0xfU << 8, 0xfU << 12, ~(0xfU << 0 | 0xfU << 4 | 0xfU << 8 | 0xfU << 12));



            //Console.WriteLine(a.Cp.ToString("X"));
            //Cube.Permute(ref a.Cp, ref x3);
            //Console.WriteLine(a.Cp.ToString("X"));
            //Cube.Permute(ref a.Cp, ref x3);
            //Console.WriteLine(a.Cp.ToString("X"));
            //Cube.Permute(ref a.Cp, ref x3);
            //Console.WriteLine(a.Cp.ToString("X"));
            //Cube.Permute(ref a.Cp, ref x3);
            //Console.WriteLine(a.Cp.ToString("X"));
            //Cube.Permute(ref a.Cp, ref x3);
            //Console.WriteLine(a.Cp.ToString("X"));

            //UInt64 cc = 0U;
            //var watch = Stopwatch.StartNew();
            //for (int i=0; i<100000000; ++i)
            //{
            //    //Cube.Orient(ref a.Co, ref rot1);
            //    Cube.Permute(ref cc, ref x4);
            //}
            //watch.Stop();
            //Console.WriteLine($"Elapsed time: {watch.ElapsedMilliseconds} ms");

            Console.ReadKey();
        }
    }
}

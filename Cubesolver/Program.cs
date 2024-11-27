using System;
using System.Diagnostics;
using static Cubesolver.NCube;

namespace Cubesolver
{
    class Program
    {

        static void Main(string[] args)
        {
            //var program = new NPathFinderProgram();
            //program.Run();
            var program = new STestProgram();
            program.Run();

            Console.ReadKey();
        }
    }
}

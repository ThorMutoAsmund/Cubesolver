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
            var n = NCube.Id;

            ConsoleKeyInfo k;
            do
            {
                Console.Clear();
                n.Display();
                k = Console.ReadKey();

                switch (k.Key)
                {
                    case ConsoleKey.U:
                        n.Turn(k.Modifiers.HasFlag(ConsoleModifiers.Shift) ? NCube.iU : NCube.tU);
                        break;
                    case ConsoleKey.F:
                        n.Turn(k.Modifiers.HasFlag(ConsoleModifiers.Shift) ? NCube.iF : NCube.tF);
                        break;
                }

            }
            while (k.Key != ConsoleKey.Escape && k.Key != ConsoleKey.Q);
        }
    }
}

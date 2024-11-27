using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubesolver
{
    using static NCube;
    public static class Visualizer
    {
        private static ConsoleColor[] ConsoleColors;

        private static ConsoleColor[] StdConsoleColors = new ConsoleColor[6] 
        {
            ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.DarkYellow
        };

        private static ConsoleColor[] ThorsConsoleColors = new ConsoleColor[6]
        {
            ConsoleColor.Red, ConsoleColor.DarkYellow, ConsoleColor.Yellow, ConsoleColor.White, ConsoleColor.Blue, ConsoleColor.Green
        };
        private static ConsoleColor bgColor;

        private static byte[,] CornerColors = new byte[8, 3] {
            { fU, fB, fL }, { fU, fR, fB }, { fU, fF, fR }, { fU, fL, fF } ,
            { fD, fL, fB }, { fD, fB, fR }, { fD, fR, fF }, { fD, fF, fL }
        };

        private static byte[,] EdgeColors = new byte[12, 2] {
            { fU, fB }, { fU, fR }, { fU, fF }, { fU, fL } ,
            { fB, fL }, { fB, fR }, { fF, fR }, { fF, fL } ,
            { fD, fB }, { fD, fR }, { fD, fF }, { fD, fL }
        };


        public static string[] TurnNames = new string[42]
        {
            "U", "U'", "F", "F'", "R", "R'", "L", "L'", "B", "B'", "D", "D'",
            "U2", "U2", "F2", "F2", "R2", "R2", "L2", "L2", "B2", "B2", "D2", "D2",
            "u", "u'","f", "f'","r", "r'","l", "l'","b", "b'","d", "d'",
            "M", "M'","E", "E'","S", "S'"
        };

        private static void DisplayCornerFace(UInt64 C, int corner, byte orientation)
        {
            var v = C >> corner;
            var p = v & 0b111;
            var o = (v & 0b11000) >> 2;
            var col = ConsoleColors[CornerColors[p, (orientation + o) % 3]];
            Console.BackgroundColor = col;
            Console.Write("  ");
            Console.BackgroundColor = bgColor;
            Console.Write(" ");
        }

        private static void DisplayEdgeFace(UInt64 E, int edge, byte orientation)
        {
            var v = E >> edge;
            var e = v & 0b1111;
            var o = (v & 0b10000) >> 4;
            var col = ConsoleColors[EdgeColors[e, orientation ^ o]];
            Console.BackgroundColor = col;
            Console.Write("  ");
            Console.BackgroundColor = bgColor;
            Console.Write(" ");
        }

        static void DisplayCenterFace(int center)
        {
            var col = ConsoleColors[center];
            Console.BackgroundColor = col;
            Console.Write("  ");
            Console.BackgroundColor = bgColor;
            Console.Write(" ");
        }

        public static void Display(UInt64 C, UInt64 E)
        {
            ConsoleColors = StdConsoleColors;
            _Display(C, E);
        }

        public static void DisplayT(UInt64 C, UInt64 E)
        {
            ConsoleColors = ThorsConsoleColors;
            _Display(C, E);
        }

        public static void _Display(UInt64 C, UInt64 E)
        {
            bgColor = Console.BackgroundColor;

            Console.Write("         ");
            DisplayCornerFace(C, p6DLB, 2);
            DisplayEdgeFace(E, p5DB, 1);
            DisplayCornerFace(C, p6DBR, 1);
            Console.WriteLine(); Console.WriteLine();

            Console.Write("         ");
            DisplayEdgeFace(E, p5BL, 0);
            DisplayCenterFace(fB);
            DisplayEdgeFace(E, p5BR, 0);
            Console.WriteLine(); Console.WriteLine();

            Console.Write("         ");
            DisplayCornerFace(C, p6UBL, 1);
            DisplayEdgeFace(E, p5UB, 1);
            DisplayCornerFace(C, p6URB, 2);
            Console.WriteLine(); Console.WriteLine();

            DisplayCornerFace(C, p6DLB, 1);
            DisplayEdgeFace(E, p5BL, 1);
            DisplayCornerFace(C, p6UBL, 2);
            DisplayCornerFace(C, p6UBL, 0);
            DisplayEdgeFace(E, p5UB, 0);
            DisplayCornerFace(C, p6URB, 0);
            DisplayCornerFace(C, p6URB, 1);
            DisplayEdgeFace(E, p5BR, 1);
            DisplayCornerFace(C, p6DBR, 2);
            DisplayCornerFace(C, p6DBR, 0);
            DisplayEdgeFace(E, p5DB, 0);
            DisplayCornerFace(C, p6DLB, 0);
            Console.WriteLine(); Console.WriteLine();

            DisplayEdgeFace(E, p5DL, 1);
            DisplayCenterFace(fL);
            DisplayEdgeFace(E, p5UL, 1);
            DisplayEdgeFace(E, p5UL, 0);
            DisplayCenterFace(fU);
            DisplayEdgeFace(E, p5UR, 0);
            DisplayEdgeFace(E, p5UR, 1);
            DisplayCenterFace(fR);
            DisplayEdgeFace(E, p5DR, 1);
            DisplayEdgeFace(E, p5DR, 0);
            DisplayCenterFace(fD);
            DisplayEdgeFace(E, p5DL, 0);
            Console.WriteLine(); Console.WriteLine();

            DisplayCornerFace(C, p6DFL, 2);
            DisplayEdgeFace(E, p5FL, 1);
            DisplayCornerFace(C, p6ULF, 1);
            DisplayCornerFace(C, p6ULF, 0);
            DisplayEdgeFace(E, p5UF, 0);
            DisplayCornerFace(C, p6UFR, 0);
            DisplayCornerFace(C, p6UFR, 2);
            DisplayEdgeFace(E, p5FR, 1);
            DisplayCornerFace(C, p6DRF, 1);
            DisplayCornerFace(C, p6DRF, 0);
            DisplayEdgeFace(E, p5DF, 0);
            DisplayCornerFace(C, p6DFL, 0);
            Console.WriteLine(); Console.WriteLine();

            Console.Write("         ");
            DisplayCornerFace(C, p6ULF, 2);
            DisplayEdgeFace(E, p5UF, 1);
            DisplayCornerFace(C, p6UFR, 1);
            Console.WriteLine(); Console.WriteLine();

            Console.Write("         ");
            DisplayEdgeFace(E, p5FL, 0);
            DisplayCenterFace(fF);
            DisplayEdgeFace(E, p5FR, 0);
            Console.WriteLine(); Console.WriteLine();

            Console.Write("         ");
            DisplayCornerFace(C, p6DFL, 1);
            DisplayEdgeFace(E, p5DF, 1);
            DisplayCornerFace(C, p6DRF, 2);
            Console.WriteLine(); Console.WriteLine();

            Console.BackgroundColor = bgColor;
        }
        
        public static List<int> FromString(string input)
        {
            var turnsAsStrings = input.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var output = new List<int>();

            foreach (var turn in turnsAsStrings)
            {
                switch (turn)
                {
                    case "U":
                        output.Add(_U);
                        break;
                    case "U'":
                        output.Add(_iU);
                        break;
                    case "U2":
                        output.Add(_U2);
                        break;

                    case "F":
                        output.Add(_F);
                        break;
                    case "F'":
                        output.Add(_iF);
                        break;
                    case "F2":
                        output.Add(_F2);
                        break;

                    case "R":
                        output.Add(_R);
                        break;
                    case "R'":
                        output.Add(_iR);
                        break;
                    case "R2":
                        output.Add(_R2);
                        break;

                    case "L":
                        output.Add(_L);
                        break;
                    case "L'":
                        output.Add(_iL);
                        break;
                    case "L2":
                        output.Add(_L2);
                        break;

                    case "B":
                        output.Add(_B);
                        break;
                    case "B'":
                        output.Add(_iB);
                        break;
                    case "B2":
                        output.Add(_B2);
                        break;

                    case "D":
                        output.Add(_D);
                        break;
                    case "D'":
                        output.Add(_iD);
                        break;
                    case "D2":
                        output.Add(_D2);
                        break;
                    default:
                        Console.WriteLine("Unkown token");
                        break;
                }
            }

            return output;
        }
    }
}

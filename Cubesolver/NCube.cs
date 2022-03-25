using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cubesolver
{
    public struct NCube //: IEquatable<NCube>
    {
        // Corner indices
        public static int pUBL = 0;
        public static int pURB = 1;
        public static int pUFR = 2;
        public static int pULF = 3;
        public static int pDLB = 4;
        public static int pDBR = 5;
        public static int pDRF = 6;
        public static int pDFL = 7;

        // Corner indices times 6
        public static int p6UBL = 0;
        public static int p6URB = 6;
        public static int p6UFR = 12;
        public static int p6ULF = 18;
        public static int p6DLB = 24;
        public static int p6DBR = 30;
        public static int p6DRF = 36;
        public static int p6DFL = 42;

        // Edge indices
        public static int pUB = 0;
        public static int pUR = 1;
        public static int pUF = 2;
        public static int pUL = 3;
        public static int pBL = 4;
        public static int pBR = 5;
        public static int pFR = 6;
        public static int pFL = 7;
        public static int pDB = 8;
        public static int pDR = 9;
        public static int pDF = 10;
        public static int pDL = 11;

        // Edge indices times 5
        public static int p5UB = 0;
        public static int p5UR = 5;
        public static int p5UF = 10;
        public static int p5UL = 15;
        public static int p5BL = 20;
        public static int p5BR = 25;
        public static int p5FR = 30;
        public static int p5FL = 35;
        public static int p5DB = 40;
        public static int p5DR = 45;
        public static int p5DF = 50;
        public static int p5DL = 55;

        // Faces
        public static byte fB = 0;
        public static byte fF = 1;
        public static byte fU = 2;
        public static byte fD = 3;
        public static byte fR = 4;
        public static byte fL = 5;

        // Turns
        public const int tU = 0;
        public const int iU = 1;
        public const int tF = 2;
        public const int iF = 3;
        public const int tR = 4;
        public const int iR = 5;
        public const int tL = 6;
        public const int iL = 7;
        public const int tB = 8;
        public const int iB = 9;
        public const int tD = 10;
        public const int iD = 11;


        public static byte[,] CornerColors = new byte[8, 3] {
            { fU, fB, fL }, { fU, fR, fB }, { fU, fF, fR }, { fU, fL, fF } ,
            { fD, fL, fB }, { fD, fB, fR }, { fD, fR, fF }, { fD, fF, fL }
        };

        public static byte[,] EdgeColors = new byte[12, 2] {
            { fU, fB }, { fU, fR }, { fU, fF }, { fU, fL } ,
            { fB, fL }, { fB, fR }, { fF, fR }, { fF, fL } ,
            { fD, fB }, { fD, fR }, { fD, fF }, { fD, fL } 
        };

        public static ConsoleColor[] ConsoleColors = new ConsoleColor[6] {
            ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.White, ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.DarkYellow
        };

        private static ConsoleColor bgColor;

        // Corner orientation:
        // 0 = U or D sticker has U or D color
        // 1 = U or D sticker has R or L color
        // 2 = U or D sticker has F or B color


        // EO definition
        // An edge is defined as oriented if it can be solved using only R, L, U and D face turns.
        // If an edge cannot be solved using these face turns then it is a misoriented or 'bad' edge.
        
        // EO detection
        // If the sticker has L/R color facing U/D it's a bad edge.
        // If the sticker has F/B color facing U/D, look at the sticker on the other side of the edge, if the side sticker has U/D color, it's a bad edge.

        // Corners
        // C0 C1 C2 C3 (ubl ubr ufr ufl)
        // C4 C5 C6 C7 (dbl dbr dfr dfl)

        // Edges
        // E0 E1 E2 E3 (ub ur uf ul)
        // E4 E5 E6 E7 (bl br fr fl)
        // E8 E9 EA EB (db dr df dl)

        //    C7        C2    C1    C0
        // OOPPP ... OOPPP OOPPP OOPPP
        UInt64 C;

        //    EB        E2    E1    E0
        // OPPPP ... OPPPP OPPPP OPPPP
        UInt64 E;

        public NCube(UInt64 c, UInt64 e)
        {
            this.C = c;
            this.E = e;
        }

        public static NCube Id = NCube.MakeId();

        public static NCube MakeId()
        {
            var c = 0LU;
            var i = 7LU;
            do
            {
                c += i;
                c <<= 6;
                i--;
            }
            while (i > 0UL);

            var e = 0LU;
            i = 11LU;
            do
            {
                e += i;
                e <<= 5;
                i--;
            }
            while (i > 0UL);

            return new NCube(c, e);
        }

        void SwapCorners(int p1, int p2)
        {
            var mask = ((1UL << 6) - 1);
            var set1 = (this.C >> p1) & mask;
            var set2 = (this.C >> p2) & mask;
            var xor = (set1 ^ set2);
            xor = (xor << p1) | (xor << p2);
            this.C = this.C ^ xor;
        }

        void SwapEdges(int p1, int p2)
        {
            var mask = ((1UL << 5) - 1);
            var set1 = (this.E >> p1) & mask;
            var set2 = (this.E >> p2) & mask;
            var xor = (set1 ^ set2);
            xor = (xor << p1) | (xor << p2);
            this.E = this.E ^ xor;
        }

        void OrientEdges(int p1, int p2, int p3, int p4)
        {
            this.E = this.E ^ ((1UL << p1 | 1UL << p2 | 1UL << p3 | 1UL << p4) << 4);
        }

        void OrientCorners_CCW(int p1, int p2)
        {
            var one = (1UL << p1 | 1UL << p2) << 3;
            var mask1 = ~(one << 2);
            var mask2 = one << 1;
            this.C = (this.C + one + ((this.C & mask2) >> 1)) & mask1;

        }

        void OrientCorners_CW(int p1, int p2)
        {
            var one = (1UL << p1 | 1UL << p2) << 3;
            var three = (3UL << p1 | 3UL << p2) << 3;
            var mask1 = ~(one << 2);
            var mask2 = one << 1;
            this.C = (this.C + three - (((this.C + three) & mask2) >>1) ) & mask1;
        }

        public void Turn(int t)
        {
            switch (t)
            {
                case tU:
                    SwapCorners(p6UBL, p6URB);
                    SwapCorners(p6UBL, p6UFR);
                    SwapCorners(p6UBL, p6ULF);
                    SwapEdges(p5UB, p5UR);
                    SwapEdges(p5UB, p5UF);
                    SwapEdges(p5UB, p5UL);
                    break;
                case iU:
                    SwapCorners(p6UBL, p6ULF);
                    SwapCorners(p6UBL, p6UFR);
                    SwapCorners(p6UBL, p6URB);
                    SwapEdges(p5UB, p5UL);
                    SwapEdges(p5UB, p5UF);
                    SwapEdges(p5UB, p5UR);
                    break;
                case tF:
                    SwapCorners(p6ULF, p6UFR);
                    SwapCorners(p6ULF, p6DRF);
                    SwapCorners(p6ULF, p6DFL);
                    SwapEdges(p5UF, p5FR);
                    SwapEdges(p5UF, p5DF);
                    SwapEdges(p5UF, p5FL);
                    OrientEdges(p5UF, p5FR, p5DF, p5FL);
                    OrientCorners_CCW(p6ULF, p6DRF);
                    OrientCorners_CW(p6UFR, p6DFL);
                    break;
                case iF:
                    SwapCorners(p6ULF, p6DFL);
                    SwapCorners(p6ULF, p6DRF);
                    SwapCorners(p6ULF, p6UFR);
                    SwapEdges(p5UF, p5FL);
                    SwapEdges(p5UF, p5DF);
                    SwapEdges(p5UF, p5FR);
                    OrientEdges(p5UF, p5FR, p5DF, p5FL);
                    OrientCorners_CW(p6ULF, p6DRF);
                    OrientCorners_CCW(p6UFR, p6DFL);  
                    break;
            }
        }

        void DisplayCornerFace(int corner, byte orientation)
        {
            var v = this.C >> corner;
            var p = v & 0b111;
            var o = (v & 0b11000)>>2;
            var col = ConsoleColors[CornerColors[p, (orientation + o) % 3]];
            Console.BackgroundColor = col;
            Console.Write("  ");
            Console.BackgroundColor = bgColor;
            Console.Write(" ");
        }

        void DisplayEdgeFace(int edge, byte orientation)
        {
            var v = this.E >> edge;
            var e = v & 0b1111;
            var o = (v & 0b10000) >> 4;
            var col = ConsoleColors[EdgeColors[e, orientation ^ o]];
            Console.BackgroundColor = col;
            Console.Write("  ");
            Console.BackgroundColor = bgColor;
            Console.Write(" ");
        }

        void DisplayCenterFace(int center)
        {
            var col = ConsoleColors[center];
            Console.BackgroundColor = col;
            Console.Write("  ");
            Console.BackgroundColor = bgColor;
            Console.Write(" ");
        }


        public void Display()
        {
            bgColor = Console.BackgroundColor;

            Console.Write("         ");
            DisplayCornerFace(p6DLB, 2);
            DisplayEdgeFace(p5DB, 1);
            DisplayCornerFace(p6DBR, 1);
            Console.WriteLine(); Console.WriteLine();
            
            Console.Write("         ");
            DisplayEdgeFace(p5BL, 0);
            DisplayCenterFace(fB);
            DisplayEdgeFace(p5BR, 0);
            Console.WriteLine(); Console.WriteLine();
            
            Console.Write("         ");
            DisplayCornerFace(p6UBL, 1);
            DisplayEdgeFace(p5UB, 1);
            DisplayCornerFace(p6URB, 2);
            Console.WriteLine(); Console.WriteLine();

            DisplayCornerFace(p6DLB, 1);
            DisplayEdgeFace(p5BL, 1);
            DisplayCornerFace(p6UBL, 2);            
            DisplayCornerFace(p6UBL, 0);
            DisplayEdgeFace(p5UB, 0);
            DisplayCornerFace(p6URB, 0);            
            DisplayCornerFace(p6URB, 1);
            DisplayEdgeFace(p5BR, 1);
            DisplayCornerFace(p6DBR, 2);            
            DisplayCornerFace(p6DBR, 0);
            DisplayEdgeFace(p5DB, 0);
            DisplayCornerFace(p6DLB, 0);
            Console.WriteLine(); Console.WriteLine();

            DisplayEdgeFace(p5DL, 1);
            DisplayCenterFace(fL);
            DisplayEdgeFace(p5UL, 1);
            DisplayEdgeFace(p5UL, 0);
            DisplayCenterFace(fU);
            DisplayEdgeFace(p5UR, 0);
            DisplayEdgeFace(p5UR, 1);
            DisplayCenterFace(fR);
            DisplayEdgeFace(p5DR, 1);
            DisplayEdgeFace(p5DR, 0);
            DisplayCenterFace(fD);
            DisplayEdgeFace(p5DL, 0);
            Console.WriteLine(); Console.WriteLine();

            DisplayCornerFace(p6DFL, 2);
            DisplayEdgeFace(p5FL, 1);
            DisplayCornerFace(p6ULF, 1);
            DisplayCornerFace(p6ULF, 0);
            DisplayEdgeFace(p5UF, 0);
            DisplayCornerFace(p6UFR, 0);
            DisplayCornerFace(p6UFR, 2);
            DisplayEdgeFace(p5FR, 1);
            DisplayCornerFace(p6DRF, 1);
            DisplayCornerFace(p6DRF, 0);
            DisplayEdgeFace(p5DF, 0);
            DisplayCornerFace(p6DFL, 0);
            Console.WriteLine(); Console.WriteLine();

            Console.Write("         ");
            DisplayCornerFace(p6ULF, 2);
            DisplayEdgeFace(p5UF, 1);
            DisplayCornerFace(p6UFR, 1);
            Console.WriteLine(); Console.WriteLine();

            Console.Write("         ");
            DisplayEdgeFace(p5FL, 0);
            DisplayCenterFace(fF);
            DisplayEdgeFace(p5FR, 0);
            Console.WriteLine(); Console.WriteLine();

            Console.Write("         ");
            DisplayCornerFace(p6DFL, 1);
            DisplayEdgeFace(p5DF, 1);
            DisplayCornerFace(p6DRF, 2);
            Console.WriteLine(); Console.WriteLine();

            Console.BackgroundColor = bgColor;
        }

    }

}

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
        public const int pUBL = 0;
        public const int pURB = 1;
        public const int pUFR = 2;
        public const int pULF = 3;
        public const int pDLB = 4;
        public const int pDBR = 5;
        public const int pDRF = 6;
        public const int pDFL = 7;

        // Corner indices times 6
        public const int p6UBL = pUBL * 6;
        public const int p6URB = pURB * 6;
        public const int p6UFR = pUFR * 6;
        public const int p6ULF = pULF * 6;
        public const int p6DLB = pDLB * 6;
        public const int p6DBR = pDBR * 6;
        public const int p6DRF = pDRF * 6;
        public const int p6DFL = pDFL * 6;

        public const UInt64 ps6UBL = 1UL << 3;
        public const UInt64 ps6URB = 1UL << 9;
        public const UInt64 ps6UFR = 1UL << 15;
        public const UInt64 ps6ULF = 1UL << 21;
        public const UInt64 ps6DLB = 1UL << 27;
        public const UInt64 ps6DBR = 1UL << 33;
        public const UInt64 ps6DRF = 1UL << 39;
        public const UInt64 ps6DFL = 1UL << 45;

        // Edge indices
        public const int pUB = 0;
        public const int pUR = 1;
        public const int pUF = 2;
        public const int pUL = 3;
        public const int pBL = 4;
        public const int pBR = 5;
        public const int pFR = 6;
        public const int pFL = 7;
        public const int pDB = 8;
        public const int pDR = 9;
        public const int pDF = 10;
        public const int pDL = 11;

        // Edge indices times 5
        public const int p5UB = pUB * 5;
        public const int p5UR = pUR * 5;
        public const int p5UF = pUF * 5;
        public const int p5UL = pUL * 5;
        public const int p5BL = pBL * 5;
        public const int p5BR = pBR * 5;
        public const int p5FR = pFR * 5;
        public const int p5FL = pFL * 5;
        public const int p5DB = pDB * 5;
        public const int p5DR = pDR * 5;
        public const int p5DF = pDF * 5;
        public const int p5DL = pDL * 5;

        // Faces
        public const byte fB = 0;
        public const byte fF = 1;
        public const byte fU = 2;
        public const byte fD = 3;
        public const byte fR = 4;
        public const byte fL = 5;

        // Turns
        public const int t0 = -1;
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
        public const int dU = 12;
        public const int DU = 13;
        public const int dF = 14;
        public const int DF = 15;
        public const int dR = 16;
        public const int DR = 17;
        public const int dL = 18;
        public const int DL = 19;
        public const int dB = 20;
        public const int DB = 21;
        public const int dD = 22;
        public const int DD = 23;


        private const UInt64 cornerMask = ((1UL << 6) - 1);
        private const UInt64 edgeMask = ((1UL << 5) - 1);

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
        // 1 = U or D sticker has color one color CCW of the U or D color
        // 2 = U or D sticker has color one color CW of the U or D color


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
            var xor = (this.C >> p1 ^ this.C >> p2) & cornerMask;
            xor = (xor << p1) | (xor << p2);
            this.C ^= xor;
        }

        void SwapEdges(int p1, int p2)
        {
            var xor = (this.E >> p1 ^ this.E >> p2) & edgeMask;
            xor = (xor << p1) | (xor << p2);
            this.E ^= xor;
        }

        void OrientEdges(int p1, int p2, int p3, int p4)
        {
            this.E ^= (1UL << p1 | 1UL << p2 | 1UL << p3 | 1UL << p4) << 4;
        }

        void OrientCorners_CCW(UInt64 p1, UInt64 p2)
        {
            var one = p1 | p2;
            var mask1 = ~(one << 2);
            var mask2 = one << 1;
            this.C = (this.C + one + ((this.C & mask2) >> 1)) & mask1;
        }

        void OrientCorners_CW(UInt64 p1, UInt64 p2)
        {
            var one = p1 | p2;
            var mask1 = ~(one << 2);
            var mask2 = one << 1;
            var three = this.C + (mask2 | one);
            this.C = (three - ((three & mask2) >> 1) ) & mask1;
        }

        void OrientCorners(UInt64 ccw1, UInt64 ccw2, UInt64 cw1, UInt64 cw2)
        {
            var ccw = ccw1 | ccw2;
            var cw = cw1 | cw2;

            var cwx2 = cw << 1;
            var three = this.C + ccw + (this.C >> 1 & ccw) + (cwx2 | cw);
            var mask = ~((cw | ccw) << 2);
            this.C = (three - ((three & cwx2) >> 1)) & mask;
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
                case tD:
                    SwapCorners(p6DLB, p6DFL);
                    SwapCorners(p6DLB, p6DRF);
                    SwapCorners(p6DLB, p6DBR);
                    SwapEdges(p5DB, p5DL);
                    SwapEdges(p5DB, p5DF);
                    SwapEdges(p5DB, p5DR);
                    break;
                case iD:
                    SwapCorners(p6DLB, p6DBR);
                    SwapCorners(p6DLB, p6DRF);
                    SwapCorners(p6DLB, p6DFL);
                    SwapEdges(p5DB, p5DR);
                    SwapEdges(p5DB, p5DF);
                    SwapEdges(p5DB, p5DL);
                    break;
                case tF:
                    SwapCorners(p6ULF, p6UFR);
                    SwapCorners(p6ULF, p6DRF);
                    SwapCorners(p6ULF, p6DFL);
                    SwapEdges(p5UF, p5FR);
                    SwapEdges(p5UF, p5DF);
                    SwapEdges(p5UF, p5FL);
                    OrientEdges(p5UF, p5FR, p5DF, p5FL);
                    OrientCorners(ps6ULF, ps6DRF, ps6UFR, ps6DFL);
                    break;
                case iF:
                    SwapCorners(p6ULF, p6DFL);
                    SwapCorners(p6ULF, p6DRF);
                    SwapCorners(p6ULF, p6UFR);
                    SwapEdges(p5UF, p5FL);
                    SwapEdges(p5UF, p5DF);
                    SwapEdges(p5UF, p5FR);
                    OrientEdges(p5UF, p5FR, p5DF, p5FL);
                    OrientCorners(ps6ULF, ps6DRF, ps6UFR, ps6DFL);
                    break;
                case tB:
                    SwapCorners(p6UBL, p6DLB);
                    SwapCorners(p6UBL, p6DBR);
                    SwapCorners(p6UBL, p6URB);
                    SwapEdges(p5UB, p5BL);
                    SwapEdges(p5UB, p5DB);
                    SwapEdges(p5UB, p5BR);
                    OrientEdges(p5UB, p5BL, p5DB, p5BR);
                    OrientCorners(ps6DLB, ps6URB, ps6UBL, ps6DBR);
                    break;
                case iB:
                    SwapCorners(p6UBL, p6URB);
                    SwapCorners(p6UBL, p6DBR);
                    SwapCorners(p6UBL, p6DLB);
                    SwapEdges(p5UB, p5BR);
                    SwapEdges(p5UB, p5DB);
                    SwapEdges(p5UB, p5BL);
                    OrientEdges(p5UB, p5BL, p5DB, p5BR);
                    OrientCorners(ps6DLB, ps6URB, ps6UBL, ps6DBR);
                    break;
                case tR:
                    SwapCorners(p6UFR, p6URB);
                    SwapCorners(p6UFR, p6DBR);
                    SwapCorners(p6UFR, p6DRF);
                    SwapEdges(p5UR, p5BR);
                    SwapEdges(p5UR, p5DR);
                    SwapEdges(p5UR, p5FR);
                    OrientCorners(ps6UFR, ps6DBR, ps6URB, ps6DRF);
                    break;
                case iR:
                    SwapCorners(p6UFR, p6DRF);
                    SwapCorners(p6UFR, p6DBR);
                    SwapCorners(p6UFR, p6URB);
                    SwapEdges(p5UR, p5FR);
                    SwapEdges(p5UR, p5DR);
                    SwapEdges(p5UR, p5BR);
                    OrientCorners(ps6UFR, ps6DBR, ps6URB, ps6DRF);
                    break;
                case tL:
                    SwapCorners(p6UBL, p6ULF);
                    SwapCorners(p6UBL, p6DFL);
                    SwapCorners(p6UBL, p6DLB);
                    SwapEdges(p5UL, p5FL);
                    SwapEdges(p5UL, p5DL);
                    SwapEdges(p5UL, p5BL);
                    OrientCorners(ps6UBL, ps6DFL, ps6ULF, ps6DLB);
                    break;
                case iL:
                    SwapCorners(p6UBL, p6DLB);
                    SwapCorners(p6UBL, p6DFL);
                    SwapCorners(p6UBL, p6ULF);
                    SwapEdges(p5UL, p5BL);
                    SwapEdges(p5UL, p5DL);
                    SwapEdges(p5UL, p5FL);
                    OrientCorners(ps6UBL, ps6DFL, ps6ULF, ps6DLB);
                    break;
                case dU:
                case DU:
                    SwapCorners(p6UBL, p6UFR);
                    SwapCorners(p6URB, p6ULF);
                    SwapEdges(p5UB, p5UF);
                    SwapEdges(p5UR, p5UL);
                    break;
                case dD:
                case DD:
                    SwapCorners(p6DLB, p6DRF);
                    SwapCorners(p6DFL, p6DBR);
                    SwapEdges(p5DB, p5DF);
                    SwapEdges(p5DL, p5DR);
                    break;
                case dF:
                case DF:
                    SwapCorners(p6ULF, p6DRF);
                    SwapCorners(p6UFR, p6DFL);
                    SwapEdges(p5UF, p5DF);
                    SwapEdges(p5FR, p5FL);
                    break;
                case dB:
                case DB:
                    SwapCorners(p6UBL, p6DBR);
                    SwapCorners(p6DLB, p6URB);
                    SwapEdges(p5UB, p5DB);
                    SwapEdges(p5BL, p5BR);
                    break;
                case dR:
                case DR:
                    SwapCorners(p6UFR, p6DBR);
                    SwapCorners(p6URB, p6DRF);
                    SwapEdges(p5UR, p5DR);
                    SwapEdges(p5BR, p5FR);
                    break;
                case dL:
                case DL:
                    SwapCorners(p6UBL, p6DFL);
                    SwapCorners(p6ULF, p6DLB);
                    SwapEdges(p5UL, p5DL);
                    SwapEdges(p5FL, p5BL);
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

        public override string ToString()
        {
            return this.GetHashCode().ToString();
        }

    }

}

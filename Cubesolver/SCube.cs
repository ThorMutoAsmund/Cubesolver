using System;
using System.Collections;
using System.Collections.Generic;

namespace Cubesolver
{     
    // A simple version of NCube
    // to be used in experiments with shaders
    public struct SCube //: IEquatable<NCube>
    {
        // Corner indices
        public const int pUBL = 0;
        public const int pURB = 1;
        public const int pUFR = 2;
        public const int pULF = 3;
        public const int pDLB = 4;
        public const int pDBR = 5;
        public const int pDRF = 6;
        public const int pDFL = 7; // Inferred

        // Corner indices times 6
        public const int p6UBL = pUBL * 6;
        public const int p6URB = pURB * 6;
        public const int p6UFR = pUFR * 6;
        public const int p6ULF = pULF * 6;
        public const int p6DLB = pDLB * 6;
        public const int p6DBR = pDBR * 6;
        public const int p6DRF = pDRF * 6;
        public const int p6DFL = pDFL * 6; // Inferred

        // Corner indices left shifted
        public const UInt64 ps6UBL = 1UL << 3;
        public const UInt64 ps6URB = 1UL << 9;
        public const UInt64 ps6UFR = 1UL << 15;
        public const UInt64 ps6ULF = 1UL << 21;
        public const UInt64 ps6DLB = 1UL << 27;
        public const UInt64 ps6DBR = 1UL << 33;
        public const UInt64 ps6DRF = 1UL << 39;
        public const UInt64 ps6DFL = 1UL << 45; // Inferred

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
        public const int pDL = 11; // Inferred

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
        public const int p5DL = pDL * 5; // Inferred

        // Faces
        public const byte fB = 0;
        public const byte fF = 1;
        public const byte fU = 2;
        public const byte fD = 3;
        public const byte fR = 4;
        public const byte fL = 5;

        // Turns
        public const int tNone = -1;
        public const int tU = 0;
        public const int t_U = 1;
        public const int tF = 2;
        public const int t_F = 3;
        public const int tR = 4;
        public const int t_R = 5;
        public const int tL = 6;
        public const int t_L = 7;
        public const int tB = 8;
        public const int t_B = 9;
        public const int tD = 10;
        public const int t_D = 11;
        public const byte tU2 = 12;
        public const int t_U2 = 13;
        public const int tF2 = 14;
        public const int t_F2 = 15;
        public const int tR2 = 16;
        public const int t_R2 = 17;
        public const int tL2 = 18;
        public const int t_L2 = 19;
        public const int tB2 = 20;
        public const int t_B2 = 21;
        public const int tD2 = 22;
        public const int t_D2 = 23;

        public static int[] AllTurns = { tU, t_U, tU2, tD, t_D, tD2, tF, t_F, tF2, tB, t_B, tB2, tR, t_R, tR2, tL, t_L, tL2 };

        private const UInt64 CornerMask = ((1UL << 6) - 1);
        private const UInt64 EdgeMask = ((1UL << 5) - 1);

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
        public UInt64 C;

        //    EB        E2    E1    E0
        // OPPPP ... OPPPP OPPPP OPPPP
        public UInt64 E;

        public SCube(UInt64 c, UInt64 e)
        {
            this.C = c;
            this.E = e;
        }

        public static SCube Id = SCube.MakeId();

        public static SCube MakeId()
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

            return new SCube(c, e);
        }

        void SwapCorners(int p1, int p2)
        {
            var xor = (this.C >> p1 ^ this.C >> p2) & CornerMask;
            xor = (xor << p1) | (xor << p2);
            this.C ^= xor;
        }

        void SwapEdges(int p1, int p2)
        {
            var xor = (this.E >> p1 ^ this.E >> p2) & EdgeMask;
            xor = (xor << p1) | (xor << p2);
            this.E ^= xor;
        }

        void OrientEdges(int f1, int f2, int f3, int f4)
        {
            this.E ^= (1UL << f1 | 1UL << f2 | 1UL << f3 | 1UL << f4) << 4;
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

        public void Turn(IEnumerable<int> turns)
        {
            foreach (var turn in turns)
            {
                Turn(turn);
            }
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
                case t_U:
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
                case t_D:
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
                case t_F:
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
                case t_B:
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
                case t_R:
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
                case t_L:
                    SwapCorners(p6UBL, p6DLB);
                    SwapCorners(p6UBL, p6DFL);
                    SwapCorners(p6UBL, p6ULF);
                    SwapEdges(p5UL, p5BL);
                    SwapEdges(p5UL, p5DL);
                    SwapEdges(p5UL, p5FL);
                    OrientCorners(ps6UBL, ps6DFL, ps6ULF, ps6DLB);
                    break;
                case tU2:
                case t_U2:
                    SwapCorners(p6UBL, p6UFR);
                    SwapCorners(p6URB, p6ULF);
                    SwapEdges(p5UB, p5UF);
                    SwapEdges(p5UR, p5UL);
                    break;
                case tD2:
                case t_D2:
                    SwapCorners(p6DLB, p6DRF);
                    SwapCorners(p6DFL, p6DBR);
                    SwapEdges(p5DB, p5DF);
                    SwapEdges(p5DL, p5DR);
                    break;
                case tF2:
                case t_F2:
                    SwapCorners(p6ULF, p6DRF);
                    SwapCorners(p6UFR, p6DFL);
                    SwapEdges(p5UF, p5DF);
                    SwapEdges(p5FR, p5FL);
                    break;
                case tB2:
                case t_B2:
                    SwapCorners(p6UBL, p6DBR);
                    SwapCorners(p6DLB, p6URB);
                    SwapEdges(p5UB, p5DB);
                    SwapEdges(p5BL, p5BR);
                    break;
                case tR2:
                case t_R2:
                    SwapCorners(p6UFR, p6DBR);
                    SwapCorners(p6URB, p6DRF);
                    SwapEdges(p5UR, p5DR);
                    SwapEdges(p5BR, p5FR);
                    break;
                case tL2:
                case t_L2:
                    SwapCorners(p6UBL, p6DFL);
                    SwapCorners(p6ULF, p6DLB);
                    SwapEdges(p5UL, p5DL);
                    SwapEdges(p5FL, p5BL);
                    break;
                
            }
        }
        public override string ToString()
        {
            return this.GetHashCode().ToString();
        }

    }
}

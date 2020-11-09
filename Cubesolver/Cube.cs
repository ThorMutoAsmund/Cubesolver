using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cubesolver
{
    public struct Permutation
    {
        public UInt64 InvMask;
        public UInt64 Mask0;
        public UInt64 Mask1;
        public UInt64 Mask2;
        public UInt64 Mask3;
        public byte Offset0;
        public byte Offset1;
        public byte Offset2;
        public byte Offset3;
    }

    public struct CornerOrientation
    {
        public UInt64 InvMask;
        public UInt64 Mask;
        public UInt64 Value;
    }

    public struct Operation
    {
        public Permutation CornerPerm;
        public Permutation EdgePerm;
        public CornerOrientation CornerOrientation;
        public UInt64 EdgeOrientation;
    }

    public enum Turn : int
    {
        U=0,  U_=1,
        F=2,  F_=3,
        R=4,  R_=5,
        L=6,  L_=7,
        B=8,  B_=9,
        D=10, D_=11,
        U2 = 12,
        F2 = 13,
        R2 = 14,
        L2 = 15,
        B2 = 16,
        D2 = 17
    }

    // UInt64 C     Corner slots - the orientation (rotation) and index of a corner in slot cs0-cs7
    //  = 0b 00111 ... 00001 00000
    //           e         e     e
    //        r  s      r  s  r  s
    //        7  7      1  1  0  0
    //
    // UInt64 E     Edge slots - the orienation (flip) and index of an edge in slots es0-esb
    // = 0b 01011 ....................... 00001 00000
    //          e                             e     e
    //      f   s                         f   s f   s
    //      b   b                         1   1 0   0
    //

    // Corners
    // 0123 (ubl ubr ufr ufl)
    // 4567 (dbl dbr dfr dfl)

    // Edges
    // 0123 (ub ur uf ul)
    // 4567 (bl br fr fl)
    // 89AB (db dr df dl)

    // Quaternions
    // http://wiki.alioth.net/index.php/Quaternion


    public struct Transform
    {
        public Turn Turn;
        public bool CO; // Corner orienting
        public bool EO; // Edge orienting
        public byte C0, C1, C2, C3;
        public byte E0, E1, E2, E3;

        public Transform(Turn turn, bool co, bool eo, byte c0, byte c1, byte c2, byte c3, byte e0, byte e1, byte e2, byte e3)
        {
            this.Turn = turn;
            this.CO = co;
            this.EO = eo;
            this.C0 = c0; this.C1 = c1; this.C2 = c2; this.C3 = c3;
            this.E0 = e0; this.E1 = e1; this.E2 = e2; this.E3 = e3;
        }
    }

    public struct Cube : IEquatable<Cube>
    {
        public UInt64 C;
        public UInt64 E;

        private static UInt64 cornerOrientationOne  = 0b0100001000010000100001000010000100001000;
        private static UInt64 cornerOrientationMask = 0b1100011000110001100011000110001100011000;

        private static Transform[] transforms = new Transform[12]
        {
            new Transform(Turn.U,  true, false, 0, 1, 2, 3,  0, 1, 2, 3),
            new Transform(Turn.U_, true, false, 0, 3, 2, 1,  0, 3, 2, 1),
            new Transform(Turn.F,  true, true,  3, 2, 6, 7,  2, 6, 10, 7),
            new Transform(Turn.F_, true, true,  3, 7, 6, 2,  2, 7, 10, 6),
            new Transform(Turn.R,  false,false, 2, 1, 5, 6,  1, 5, 9, 6),
            new Transform(Turn.R_, false,false, 2, 6, 5, 1,  1, 6, 9, 5),
            new Transform(Turn.L,  false,false, 0, 3, 7, 4,  3, 7, 11, 4),
            new Transform(Turn.L_, false,false, 0, 4, 7, 3,  3, 4, 11, 7),
            new Transform(Turn.B,  true, true,  1, 0, 4, 5,  0, 4, 8, 5),
            new Transform(Turn.B_, true, true,  1, 5, 4, 0,  0, 5, 8, 4),
            new Transform(Turn.D,  true, false, 7, 6, 5, 4,  10, 9, 8, 11),
            new Transform(Turn.D_, true, false, 7, 4, 5, 6,  10, 11, 8, 9),
        };

        private static Operation[] operations = MakeOperations();

        private Cube(UInt64 c, UInt64 e)
        {
            this.C = c;
            this.E = e;
        }

        public static Cube Id = Cube.MakeId();

        private static Cube MakeId()
        {
            var shift = 0;
            var cube = new Cube(0U, 0U);
            for (UInt64 i = 0; i < 8; ++i)
            {
                cube.C += i << shift;
                cube.E += i << shift;
                shift += 5;
            }
            for (UInt64 i = 8; i < 12; ++i)
            {
                cube.E += i << shift;
                shift += 5;
            }
            return cube;
        }

        private static Operation[] MakeOperations()
        {
            var op = new Operation[12];

            for (int i = 0; i < transforms.Length; ++i)
            {
                var tr = transforms[i];
                op[i].CornerPerm.InvMask = 0U; // Hertil
            }

            return op;
        }


        public bool Equals(Cube other)
        {
            return other.C == this.C && other.E == this.E;
        }

        public bool IsIdentity => this.Equals(Id);

        /// Operations
        /// 
        public static void Perform(ref UInt64 o, ref UInt64 e, ref Operation operation)
        {
            Permute(ref o, ref operation.CornerPerm);
            Orient(ref o, ref operation.CornerOrientation);
            Permute(ref e, ref operation.EdgePerm);
            Flip(ref e, ref operation.EdgeOrientation);
        }

        public static void Orient(ref UInt64 o, ref CornerOrientation orientation)
        {
            o = (o & orientation.InvMask) + (o + orientation.Value + ((o + orientation.Value) >> 1) + cornerOrientationOne) & cornerOrientationMask;
        }

        public static void Flip(ref UInt64 e, ref UInt64 orientation)
        {
            e ^= orientation;
        }

        public static void Permute(ref UInt64 c, ref Permutation perm)
        {
            c = c & perm.InvMask |
            ((c & perm.Mask0) << perm.Offset0) |
            ((c & perm.Mask1) << perm.Offset1) |
            ((c & perm.Mask2) << perm.Offset2) |
            ((c & perm.Mask3) >> perm.Offset3);
        }

        //public static void OldPermute(ref UInt32 Cp, ref (byte, byte, byte, byte, byte, byte, byte, byte, UInt32, UInt32, UInt32, UInt32, UInt32) input)
        //{
        //    Cp = Cp & input.Item13 |
        //    ((Cp & input.Item9) << input.Item5) |
        //    ((Cp & input.Item10) << input.Item6) |
        //    ((Cp & input.Item11) << input.Item7) |
        //    ((Cp & input.Item12) >> input.Item8);
        //}

        //public static void OldPermute(ref UInt64 Ep, ref (byte, byte, byte, byte, byte, byte, byte, byte, UInt64, UInt64, UInt64, UInt64, UInt64) input)
        //{
        //    Ep = Ep & input.Item13 |
        //    ((Ep & input.Item9) << input.Item5) |
        //    ((Ep & input.Item10) << input.Item6) |
        //    ((Ep & input.Item11) << input.Item7) |
        //    ((Ep & input.Item12) >> input.Item8);
        //}

        public void DisplayBinary()
        {
            Console.Write(Convert.ToString((UInt32)((this.C >> 32) & 0xFFFFFFFF), 2).PadLeft(32, '0'));
            Console.Write(" ");
            Console.WriteLine(Convert.ToString((UInt32)(this.C & 0xFFFFFFFF), 2).PadLeft(32, '0'));
            Console.Write(Convert.ToString((UInt32)((this.E >> 32) & 0xFFFFFFFF), 2).PadLeft(32, '0'));
            Console.Write(" ");
            Console.WriteLine(Convert.ToString((UInt32)(this.E & 0xFFFFFFFF), 2).PadLeft(32, '0'));

        }
    }
}

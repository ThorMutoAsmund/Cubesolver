using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cubesolver.NCube;

namespace Cubesolver
{
    public class NPathFinder
    {
        public const int trNN = -1;
        public const int tr00 = 0;   // R U R'
        public const int tr01 = 1;   // R U2 R'
        public const int tr02 = 2;   // R U' R'
        public const int tr03 = 3;   // R' U' R
        public const int tr04 = 4;   // R' U R
        public const int tr05 = 5;   // R' U2 R
        public const int tr06 = 6;   // R' D R
        public const int tr07 = 7;   // R' D' R
        public const int tr08 = 8;   // R D R'
        public const int tr09 = 9;   // R D' R'
        public const int tr10 = 10; // R' U' R'
        public const int tr11 = 11; // R' U R'
        public const int tr12 = 12; // R U' R
        public const int tr13 = 13; // R U R
        public const int tr14 = 14; // R2 D' R2
        public const int tr15 = 15; // R2 D R2
        public const int tr16 = 16; // R' F R
        public const int tr17 = 17; // R' F' R'
        public const int tr18 = 18; // F
        public const int tr19 = 19; // F'
        public const int tr20 = 20; // R
        public const int tr21 = 21; // R'
        public const int tr22 = 22; // U
        public const int tr23 = 23; // U'
        public const int tr24 = 24; // U2
        public const int tr25 = 25; // D
        public const int tr26 = 26; // D'
        public const int numtriggers = tr26 + 1;

        private static int[,] triggerMoves = 
        {
            {tR, tU, iR, t0},
            {tR, dU, iR, t0},
            {tR, iU, iR, t0},
            {iR, iU, tR, t0},
            {iR, tU, tR, t0},
            {iR, dU, tR, t0},
            {iR, tD, tR, t0},
            {iR, iD, tR, t0},
            {tR, tD, iR, t0},
            {tR, iD, iR, t0},
            {iR, iU, iR, t0},
            {iR, tU, iR, t0},
            {tR, iU, tR, t0},
            {tR, tU, tR, t0},
            {dR, iD, dR, t0},
            {dR, tD, dR, t0},
            {iR, tF, tR, t0},
            {iR, iF, iR, t0},
            {tF, t0, t0, t0},
            {iF, t0, t0, t0},
            {tR, t0, t0, t0},
            {iR, t0, t0, t0},
            {tU, t0, t0, t0},
            {iU, t0, t0, t0},
            {dU, t0, t0, t0},
            {tD, t0, t0, t0},
            {iD, t0, t0, t0},
        };

        private static int[,] allowedTriggers =
        {
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr08, tr09, tr12, tr13, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr06, tr07, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr06, tr07, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr18, tr19, tr20, tr21, tr22, tr23, tr24, tr25, tr26},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr18, tr19, tr20, tr21, tr22, tr23, tr24, tr25, tr26},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr22, tr23, tr24, tr25, tr26},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr22, tr23, tr24, tr25, tr26},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr20, tr21, tr22, tr23, tr24, tr25, tr26},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr20, tr21, tr22, tr23, tr24, tr25, tr26},
            {tr00, tr01, tr02, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr25, tr26, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr25, tr26, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr25, tr26, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr22, tr23, tr24},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr22, tr23, tr24},
        };

        private static int[] startTriggers = {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, 
            tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr22, tr23, tr24, tr25, tr26 };


        public void GenerateBaseSet(int depth)
        {
            var q = new Queue<NCube>();
            q.Enqueue(NCube.Id);

            while (q.Count > 0)
            {
                //q.Dequeue
            }
        }
    }
}




// Tag en løst cube og lave alle kombinationer af triggers til en hvis dybde og gem dem

// Tag en scramblet cube og se om man kan ramme en af dem der er gemt.

/*
def get_dist_6():
    dist = { }
            i = 0
    # q = deque([(k, [], 'k')])
    q = deque()
    for solved_case in solved:
        # print(solved_case)
        q.append((solved_case, [], 'k'))
    while q:
        s, path, last_trigger = q.popleft()
        if len(path) < 5:
            for trigger in triggers[last_trigger]:
                npos = s
                for move in trigger.split(' '):
                    npos = mdic[move](npos)
                #print(self.pos)
                newpath = copy(path)
                newpath.append(trigger)
                if npos not in dist:
            dist[npos] = newpath
                    q.append((npos, newpath, trigger))
                    i += 1
                    if i % 100000 == 0:
                        print(i, len(newpath))
        else:
            break
    return dist
*/

/*
triggers = {

"R U R'":["R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R' U' R'","R' U R'","R2 D' R2","R2 D R2","F","F'","R","U","U'","U2","D","D'"],
"R U2 R'":["R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R' U' R'","R' U R'","F","F'","R","U","U'","U2","D","D'"],
"R U' R'":["R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R' U' R'","R' U R'","R2 D' R2","R2 D R2","F","F'","R","U","U'","U2","D","D'"],
"R' U' R":["R U R'","R U2 R'","R U' R'","R' F R","R' F' R'","R D R'","R D' R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","U","U'","U2","D","D'"],
"R' U R":["R U R'","R U2 R'","R U' R'","R' F R","R' F' R'","R D R'","R D' R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","U","U'","U2","D","D'"],
"R' U2 R":["R U R'","R U2 R'","R U' R'","R' F R","R' F' R'","R D R'","R D' R'","R U' R","R U R","F","F'","R","U","U'","U2","D","D'"],
"R' D R":["R U R'","R U2 R'","R U' R'","R' F R","R' F' R'","R D R'","R D' R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","U","U'","U2","D","D'"],
"R' D' R":["R U R'","R U2 R'","R U' R'","R' F R","R' F' R'","R D R'","R D' R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","U","U'","U2","D","D'"],
"R D R'":["R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R' U' R'","R' U R'","R2 D' R2","R2 D R2","F","F'","R'","U","U'","U2","D","D'"],
"R D' R'":["R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R' U' R'","R' U R'","R2 D' R2","R2 D R2","F","F'","R'","U","U'","U2","D","D'"],
"R' U' R'":["R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R' U' R'","R' U R'","R2 D' R2","R2 D R2","F","F'","R'","U","U'","U2","D","D'"],
"R' U R'":["R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R' U' R'","R' U R'","R2 D' R2","R2 D R2","F","F'","R'","U","U'","U2","D","D'"],
"R U' R":["R U R'","R U2 R'","R U' R'","R' F R","R' F' R'","R' D R","R' D' R","R D R'","R D' R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","U","U'","U2","D","D'"],
"R U R":["R U R'","R U2 R'","R U' R'","R' F R","R' F' R'","R' D R","R' D' R","R D R'","R D' R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","U","U'","U2","D","D'"],
"R2 D' R2":["R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R","F","F'","R","R'","U","U'","U2","D","D'"],
"R2 D R2":["R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R","F","F'","R","R'","U","U'","U2","D","D'"],
"R' F R":["R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","R'","U","U'","U2","D","D'"],
"R' F' R'":["R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","R'","U","U'","U2","D","D'"],
"F":["R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R","R2 D' R2","R2 D R2","R","R'","U","U'","U2","D","D'"],
"F'":["R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R","R2 D' R2","R2 D R2","R","R'","U","U'","U2","D","D'"],
"R":["R U R'","R U2 R'","R U' R'","R D R'","R D' R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","U","U'","U2","D","D'"],
"R'":["R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R' U' R'","R' U R'","R2 D' R2","R2 D R2","F","F'","R'","U","U'","U2","D","D'"],
"U":["R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","R'","D","D'"],
"U'":["R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","R'","D","D'"],
"U2":["R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","R'","D","D'"],
"D":["R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","R'","U","U'","U2"],
"D'":["R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","R'","U","U'","U2"],
"k":["R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' F R","R' F' R'","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R","R2 D' R2","R2 D R2","F","F'","R","R'","U","U'","U2","D","D'"]}
*/
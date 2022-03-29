using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cubesolver.NCube;

namespace Cubesolver
{
    public class NPathFinder
    {
        public const byte trNN = 0;
        public const byte tr00 = 1;   // R U R'
        public const byte tr01 = 2;   // R U2 R'
        public const byte tr02 = 3;   // R U' R'
        public const byte tr03 = 4;   // R' U' R
        public const byte tr04 = 5;   // R' U R
        public const byte tr05 = 6;   // R' U2 R
        public const byte tr06 = 7;   // R' D R
        public const byte tr07 = 8;   // R' D' R
        public const byte tr08 = 9;   // R D R'
        public const byte tr09 = 10;   // R D' R'
        public const byte tr10 = 11; // R' U' R'
        public const byte tr11 = 12; // R' U R'
        public const byte tr12 = 13; // R U' R
        public const byte tr13 = 14; // R U R
        public const byte tr14 = 15; // R2 D' R2
        public const byte tr15 = 16; // R2 D R2
        public const byte tr16 = 17; // R' F R
        public const byte tr17 = 18; // R' F' R'
        public const byte tr18 = 19; // F
        public const byte tr19 = 20; // F'
        public const byte tr20 = 21; // R
        public const byte tr21 = 22; // R'
        public const byte tr22 = 23; // U
        public const byte tr23 = 24; // U'
        public const byte tr24 = 25; // U2
        public const byte tr25 = 26; // D
        public const byte tr26 = 27; // D'
        public const byte numTriggers = 27; // D'

        private static string[] triggerNames = 
            {
            "", "R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R",
            "R2 D' R2","R2 D R2","R' F R","R' F' R'","F","F'","R","R'","U","U'","U2","D","D'"
            };


        private static int[,] triggerMoves = 
        {
            {t0, t0, t0, t0},
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

        private static byte[,] allowedTriggers =
        {
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr22, tr23, tr24, tr25, tr26 },
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr08, tr09, tr12, tr13, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr06, tr07, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr16, tr17, tr06, tr07, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr18, tr19, tr20, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr18, tr19, tr20, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr20, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr20, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN},
            {tr00, tr01, tr02, tr08, tr09, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr10, tr11, tr14, tr15, tr18, tr19, tr21, tr22, tr23, tr24, tr25, tr26, trNN, trNN, trNN, trNN, trNN, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr25, tr26, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr25, tr26, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr25, tr26, trNN, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr22, tr23, tr24, trNN, trNN},
            {tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr22, tr23, tr24, trNN, trNN},
        };

        public int Size => this.nstore.Count;
        public int KeyStoreSize => this.keystore.Count;

        private Dictionary<int, List<byte[]>> nstore = new Dictionary<int, List<byte[]>>();
        private HashSet<int> keystore = new HashSet<int>();

        public void GenerateBaseSet(int depth)
        {
            this.nstore.Clear();
            this.keystore.Clear();

            var cube = NCube.Id;
            nstore[cube.GetHashCode()] = new List<byte[]>() { new byte[0] };

            if (depth <= 0)
            {
                return;
            }

            int PerformTrigger(int t)
            {
                for (int i = 0; i < 4; ++i)
                {
                    var turn = triggerMoves[t, i];
                    if (turn == t0)
                    {
                        return i - 1;
                    }
                    cube.Turn(turn);
                }
                return 3;
            }

            void PerformInverseTrigger(int t, int iStart)
            {
                for (int i = iStart; i >= 0; --i)
                {
                    var turn = triggerMoves[t, i];
                    cube.Turn(turn ^ 1);
                }
            }

            void Rec(int lastTrigger, int level, byte[] lastPath)
            {
                for (int i = 0; i < numTriggers; ++i)
                {
                    var trigger = allowedTriggers[/*lastTrigger*/0, i];
                    if (trigger == trNN)
                    {
                        break;
                    }

                    var max = PerformTrigger(trigger);
                    var key = cube.GetHashCode();
#if STORE
                    var path = new byte[lastPath.Length+1];
                    Array.Copy(lastPath, path, lastPath.Length);
                    path[lastPath.Length] = trigger;
                    
                    if (!nstore.ContainsKey(key))
                    {
                        nstore[key] = new List<byte[]>() { path };
                    }
                    else
                    {
                        nstore[key].Add(path);
                    }
                    if (level < depth)
                    {
                        Rec(trigger, level + 1, path);
                    }
#else
                    if (!this.keystore.Contains(key))
                    {
                        this.keystore.Add(key);
                        if (level < depth - 1)
                        {
                            Rec(trigger, level + 1, lastPath);
                        }
                    }

#endif

                    PerformInverseTrigger(trigger, max);
                }
            }

            Rec(trNN, 0, new byte[0]);
        }

        public void GenerateFile()
        {
            var fullPath = DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".txt";
            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                foreach (var pair in nstore)
                {
                    writer.Write($"{pair.Key}: ");
                    foreach (var trigger in pair.Value)
                    {
                        writer.Write($"{String.Join(", ", trigger.Select(a => triggerNames[a]))}  |  ");
                    }
                    writer.WriteLine();

                }
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
# print(self.pos)
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
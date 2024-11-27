
// Ide!
// Når en ny key er fundet, så gem keyen og et index til det triggerset der skabte den
// det triggerset har en pointer til NIL
// Når en eksisterende key findes, så gemmes det nye triggersæt og det forrige triggersæt sættes til at pege på dette
// Således undgås lister/arrays


using System;
using System.Collections.Generic;
using System.Linq;
using static Cubesolver.NCube;

namespace Cubesolver
{
    public class TriggerSetRef
    {
        public UInt64 TriggerSet { get; set; }
        public TriggerSetRef NextTriggerSet { get; set; }

        public TriggerSetRef(UInt64 triggerSet)
        {
            this.TriggerSet = triggerSet;
            this.NextTriggerSet = null;
        }

        public TriggerSetRef(UInt64 triggerSet, TriggerSetRef prev)
        {
            this.TriggerSet = triggerSet;
            this.NextTriggerSet = prev;
        }
    }

    public class NPathFinder
    {
        public const byte trNN = 0;
        public const byte tr00 = 1;  // R U R'
        public const byte tr01 = 2;  // R U2 R'
        public const byte tr02 = 3;  // R U' R'
        public const byte tr03 = 4;  // R' U' R
        public const byte tr04 = 5;  // R' U R
        public const byte tr05 = 6;  // R' U2 R
        public const byte tr06 = 7;  // R' D R
        public const byte tr07 = 8;  // R' D' R
        public const byte tr08 = 9;  // R D R'
        public const byte tr09 = 10; // R D' R'
        public const byte tr10 = 11; // R' U' R'
        public const byte tr11 = 12; // R' U R'
        public const byte tr12 = 13; // R U' R
        public const byte tr13 = 14; // R U R
        public const byte tr14 = 15; // R2 D' R2
        public const byte tr15 = 16; // R2 D R2
        public const byte tr16 = 17; // R' F R
        public const byte tr17 = 18; // R' F' R
        public const byte tr18 = 19; // F
        public const byte tr19 = 20; // F'
        public const byte tr20 = 21; // R
        public const byte tr21 = 22; // R'
        public const byte tr22 = 23; // U
        public const byte tr23 = 24; // U'
        public const byte tr24 = 25; // U2
        public const byte tr25 = 26; // D
        public const byte tr26 = 27; // D'
        
        public const byte tr27 = 28; // r U r'
        public const byte tr28 = 29; // r U' r'
        public const byte tr29 = 30; // r' U' r
        public const byte tr30 = 31; // r' U r
        public const byte tr31 = 32; // R2 U R2
        public const byte tr32 = 33; // R2 U' R2
        public const byte tr33 = 34; // r
        public const byte tr34 = 35; // r'
        public const byte tr35 = 36; // S
        public const byte tr36 = 37; // S'

        public const byte numTriggers = 37;

        private static string[] triggerNames =
            {
            "", "R U R'","R U2 R'","R U' R'","R' U' R","R' U R","R' U2 R","R' D R","R' D' R","R D R'","R D' R'","R' U' R'","R' U R'","R U' R","R U R",
            "R2 D' R2","R2 D R2","R' F R","R' F' R","F","F'","R","R'","U","U'","U2","D","D'",
            "r U r'","r U' r'","r' U' r","r' U r","R2 U R2","R2 U' R2", "r", "r'", "S", "S'"
            };
        private static string[] inverseTriggerNames =
            {
            "", "R U' R'","R U2 R'","R U R'","R' U R","R' U' R","R' U2 R","R' D' R","R' D R","R D' R'","R D R'","R U R","R U' R","R' U R'","R' U' R'",
            "R2 D R2","R2 D' R2","R' F' R","R' F R","F'","F","R'","R","U'","U","U2","D'","D",
            "r U' r'","r U r'","r' U r","r' U' r","R2 U' R2","R2 U R2", "r'", "r", "S'", "S"
            };


        private static int[,] triggerMoves = 
        {
            {_0, _0, _0, _0},
            {_R, _U, _iR, _0},
            {_R, _U2, _iR, _0},
            {_R, _iU, _iR, _0},
            {_iR, _iU, _R, _0},
            {_iR, _U, _R, _0},
            {_iR, _U2, _R, _0},
            {_iR, _D, _R, _0},
            {_iR, _iD, _R, _0},
            {_R, _D, _iR, _0},
            {_R, _iD, _iR, _0},
            {_iR, _iU, _iR, _0},
            {_iR, _U, _iR, _0},
            {_R, _iU, _R, _0},
            {_R, _U, _R, _0},
            {_R2, _iD, _R2, _0},
            {_R2, _D, _R2, _0},
            {_iR, _F, _R, _0},
            {_iR, _iF, _R, _0},
            {_F, _0, _0, _0},
            {_iF, _0, _0, _0},
            {_R, _0, _0, _0},
            {_iR, _0, _0, _0},
            {_U, _0, _0, _0},
            {_iU, _0, _0, _0},
            {_U2, _0, _0, _0},
            {_D, _0, _0, _0},
            {_iD, _0, _0, _0},

            {_wR, _U, _iwR, _0},
            {_wR, _iU, _iwR, _0},
            {_iwR, _iU, _wR, _0},
            {_iwR, _U, _wR, _0},

            {_R2, _U, _R2, _0},
            {_R2, _iU, _R2, _0},
            {_wR, _0, _0, _0},
            {_iwR, _0, _0, _0},
            {_S, _0, _0, _0},
            {_iS, _0, _0, _0},
        };

        private static byte[] allTriggers = { tr00, tr01, tr02, tr03, tr04, tr05, tr16, tr17, tr06, tr07, tr08, tr09, tr10, tr11, tr12, tr13, tr14, tr15, tr18, tr19, tr20, tr21, tr22, tr23, tr24, tr25, tr26, tr27, tr28, tr29, tr30, tr31, tr32, tr33, tr34, tr35, tr36 };
        public bool StoreOnlyFirstMatchingTriggerSet { get; set; } = false;
        public int KeyStoreSize => this.baseSet.Count;

        private Dictionary<NCube, TriggerSetRef> baseSet = new Dictionary<NCube, TriggerSetRef>();
        private List<NSolution> solutions = new List<NSolution>();

        private NCube cube = NCube.Id;
        private NCube bcube = NCube.Id;
        private NCube targetCase;

        public const UInt64 search1 = (tr32 << 24) | (tr00 << 18) | (tr11 << 12) | (tr23 << 6) | tr20;
        public const UInt64 search2 = (tr32 << 18) | (tr00 << 12) | (tr11 << 6) | (tr23 << 0);
        public const UInt64 search2b = (tr32 << 24) | (tr00 << 18) | (tr11 << 12) | (tr23 << 6);

        public NPathFinder(bool storeOnlyFirstMatchingTriggerSet = false)
        {
            this.StoreOnlyFirstMatchingTriggerSet = storeOnlyFirstMatchingTriggerSet;
        }

        private int PerformTrigger(UInt32 t)
        {
            for (int i = 0; i < 4; ++i)
            {
                var turn = triggerMoves[t, i];
                if (turn == _0)
                {
                    return i - 1;
                }
                this.cube.Turn(turn);
            }
            return 3;
        }

        private void PerformInverseTrigger(UInt32 t, int iStart)
        {
            for (int i = iStart; i >= 0; --i)
            {
                var turn = triggerMoves[t, i];
                this.cube.Turn(turn ^ 1);
            }
        }

        // Rule out "overloaded R" and "F followed by M"

        public void GenerateBaseSet(int depth)
        {
            Console.WriteLine("Generating base set...");

            this.baseSet.Clear();

            this.baseSet[cube] = new TriggerSetRef(trNN);

            if (depth <= 0)
            {
                return;
            }

            var queue = new List<UInt32>();
            queue.Add((0 << 6) | trNN);
            UInt32 qptr = 0;

            while (qptr < queue.Count)
            {
                int level = 0;
                this.cube = NCube.Id;

                var trs = new List<uint>();
                UInt32 prev = queue[(int)qptr++];
                UInt32 trigger = prev & 63;
                prev >>= 6;
                UInt32 lastTrigger = trigger;
                while (trigger != trNN)
                {
                    trs.Insert(0,trigger);
                    lastTrigger = trigger;
                    level++;
                    if (prev == 0)
                    {
                        break;
                    }
                    prev = queue[(int)prev];
                    trigger = prev & 63;
                    prev >>= 6;
                }

                UInt64 triggerSet = trNN;
                foreach (var t in trs)
                {
                    triggerSet = (triggerSet | t) << 6;
                    PerformTrigger(t);
                }

                this.bcube = this.cube;

                for (int i = 0; i < numTriggers; ++i)
                {
                    trigger = allTriggers[i];
                    if (trigger == trNN)
                    {
                        break;
                    }

                    PerformTrigger(trigger);

                    if (!this.baseSet.ContainsKey(this.cube))
                    {
                        this.baseSet[this.cube] = new TriggerSetRef(triggerSet | trigger);

                        if (level < depth - 1)
                        {
                            queue.Add(((qptr - 1) << 6) | trigger);
                        }
                    }
                    else
                    {
                        if (!this.StoreOnlyFirstMatchingTriggerSet)
                        {
                            this.baseSet[this.cube] = new TriggerSetRef(triggerSet | trigger, this.baseSet[this.cube]);
                        }
                    }

                    this.cube = bcube;
                }
            }

            var process = System.Diagnostics.Process.GetCurrentProcess();
            process.Refresh();
            Console.WriteLine($"MemoryUsed = {process.WorkingSet64 / (1024)} kB");
        }

        public void PrintBaseSet()
        {
            foreach (var entry in this.baseSet)
            {
                var v = entry.Value;
                string s = TriggerSetToString(v.TriggerSet);
                while (v.NextTriggerSet != null)
                {
                    v = v.NextTriggerSet;
                    s = TriggerSetToString(v.TriggerSet) + ", " + s;
                }
                Console.WriteLine($"{entry.Key.C}-{entry.Key.E}: {s}");
            }
        }

        public void FindPathsToCase(int depth)
        {
            Console.WriteLine("Finding solutions...");
            this.solutions = new List<NSolution>();

            if (this.baseSet.ContainsKey(this.targetCase))
            {
                AddSolution(this.baseSet[this.targetCase], trNN);
            }

            var queue = new List<UInt32>();
            queue.Add((trNN << 6) | trNN);
            UInt32 qptr = 0;

            while (qptr < queue.Count)
            {
                int level = 0;
                this.cube = this.targetCase;

                var trs = new List<uint>();
                UInt32 prev = queue[(int)qptr++];
                UInt32 trigger = prev & 63;
                prev >>= 6;
                UInt32 lastTrigger = trigger;
                while (trigger != trNN)
                {
                    trs.Insert(0, trigger);
                    lastTrigger = trigger;
                    level++;
                    if (prev == 0)
                    {
                        break;
                    }
                    prev = queue[(int)prev];
                    trigger = prev & 63;
                    prev >>= 6;
                }

                UInt64 triggerSet = trNN;
                foreach (var t in trs)
                {
                    triggerSet = (triggerSet | t) << 6;
                    PerformTrigger(t);
                }

                this.bcube = this.cube;

                for (int i = 0; i < numTriggers; ++i)
                {
                    trigger = allTriggers[i];

                    PerformTrigger(trigger);

                    if (this.baseSet.ContainsKey(this.cube))
                    {
                        AddSolution(this.baseSet[this.cube], triggerSet | trigger);
                    }

                    if (level < depth - 1)
                    {
                        queue.Add(((qptr - 1) << 6) | trigger);
                    }

                    this.cube = bcube;
                }
            }

            Console.WriteLine("Finished");
        }

        private void AddSolution(TriggerSetRef leftSets, UInt64 right)
        {
            var rightTurns = new List<byte>();

            while (right != 0)
            {
                var idx = (byte)(right & 63);
                for (int i = 3; i >= 0; --i)
                {
                    var move = triggerMoves[idx, i];
                    if (move == _0)
                    {
                        continue;
                    }
                    rightTurns.Insert(0, (byte)move);
                }
                right >>= 6;
            }

            while (leftSets != null)
            {
                var left = leftSets.TriggerSet;
                leftSets = leftSets.NextTriggerSet;

                var turns = new List<byte>(rightTurns);
                
                while (left != 0)
                {
                    var idx = (byte)(left & 63);
                    for (int i = 3; i >= 0; --i)
                    {
                        var move = triggerMoves[idx, i];
                        if (move == _0)
                        {
                            continue;
                        }
                        turns.Add((byte)(move ^ 1));
                    }
                    left >>= 6;
                }

                // Remove things like R R  or U' U2

                // TBD D' U D , D U D' etc
                do
                {
                    int t = 0;
                    var culled = new List<byte>();
                    while (t < turns.Count)
                    {
                        if (turns[t] > _U2 && turns[t] < _wU)
                        {
                            turns[t] = (byte)(turns[t] & ~1);
                        }

                        if (t == turns.Count - 1)
                        {
                            culled.Add(turns[t]);
                            break;
                        }

                        if (turns[t] == (turns[t + 1] ^ 1))
                        {
                            t += 2;
                        }
                        else if (turns[t] == turns[t + 1])
                        {
                            if (turns[t] < _U2)
                            {
                                culled.Add((byte)(_U2 + (turns[t] & ~1)));
                                t += 2;
                            }
                            else if (turns[t] < _wU)
                            {
                                t += 2;
                            }
                            else
                            {
                                culled.Add(turns[t]);
                                t += 1;
                            }
                        }
                        else if (turns[t] < _U2 && (turns[t] & ~1) + _U2 == (turns[t + 1] & ~1))
                        {
                            culled.Add((byte)(turns[t] ^ 1));
                            t += 2;
                        }
                        else if (turns[t + 1] < _U2 && (turns[t + 1] & ~1) + _U2 == (turns[t] & ~1))
                        {
                            culled.Add((byte)(turns[t + 1] ^ 1));
                            t += 2;
                        }
                        else
                        {
                            culled.Add(turns[t]);
                            t += 1;
                        }
                    }

                    if (culled.Count == turns.Count)
                    {
                        break;
                    }
                    turns = culled;
                }
                while (true);

                var solution = new NSolution(turns.ToArray());
                if (!this.solutions.Contains(solution))
                {
                    this.solutions.Add(solution);

                    foreach (var turn in turns)
                    {
                        Console.Write(Visualizer.TurnNames[turn] + " ");
                    }

                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
        }

        // Fix D U D or D U2 D or D U2 D2 or D U D' etc

        public static string TriggerSetToInverseString(UInt64 triggerSet)
        {
            // 68
            string triggerString = string.Empty;
            while (triggerSet != 0)
            {
                triggerString = triggerString + " - " + inverseTriggerNames[triggerSet & 63];
                triggerSet >>= 6;
            }
            return triggerString;
        }

        public static string TriggerSetToString(UInt64 triggerSet)
        {
            // 68
            string triggerString = string.Empty;
            if (triggerSet != 0)
            {
                triggerString = triggerNames[triggerSet & 63];
                triggerSet >>= 6;
                while (triggerSet != 0)
                {
                    triggerString = triggerNames[triggerSet & 63] + " - " + triggerString;
                    triggerSet >>= 6;
                }
            }

            return triggerString;
        }

        //private void DisplayAlg(int key1, UInt64 left, UInt64 right)
        //{
        //    Console.WriteLine($"[{key1}] {TriggerSetToString(right)}  >>  {TriggerSetToInverseString(left)}");

        //    if (++this.algsDisplayed % 16 == 0)
        //    {
        //        Console.ReadKey();
        //    }
        //}

        public void SetCaseFromInverse(int[] turns)
        {
            this.targetCase = NCube.Id;
            foreach (var turn in turns.Reverse())
            {
                this.targetCase.Turn(turn ^ 1);
            }
        }

        public void GenerateFile()
        {
            //var fullPath = DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".txt";
            //using (StreamWriter writer = new StreamWriter(fullPath))
            //{
            //    foreach (var pair in nstore)
            //    {
            //        writer.Write($"{pair.Key}: ");
            //        foreach (var trigger in pair.Value)
            //        {
            //            writer.Write($"{String.Join(", ", trigger.Select(a => triggerNames[a]))}  |  ");
            //        }
            //        writer.WriteLine();

            //    }
            //}
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


            void DFS(int llevel)//, int lastTrigger, byte[] lastPath)
            {
                for (int i = 0; i < numTriggers; ++i)
                {
                    var trigger = allowedTriggers[lastTrigger0, i]; // at least 559 with depth 2
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
                    if (llevel < depth)
                    {
                        Rec(trigger, llevel + 1, path);
                    }
#else
if (!this.keystore.ContainsKey(key))
{
    this.keystore[key] = llevel;
    if (llevel < depth - 1)
    {
        DFS(llevel + 1);//trigger, , lastPath);
    }
}
else if (this.keystore[key] > llevel)
{
    this.keystore[key] = llevel;
    DFS(llevel + 1);//trigger, , lastPath);
}
#endif

PerformInverseTrigger(trigger, max);
                }
            }

*/
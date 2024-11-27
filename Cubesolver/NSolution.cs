using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubesolver
{
    public struct NSolution
    {
        public byte[] Turns { get; set; }
        public NSolution(byte[] turns)
        {
            this.Turns = turns;
        }

        public override bool Equals(object ob)
        {
            if (ob is NSolution)
            {
                NSolution c = (NSolution)ob;
                if (c.Turns.Length != this.Turns.Length)
                {
                    return false;
                }
                for (int t = 0; t < this.Turns.Length; ++t)
                {
                    if (this.Turns[t] != c.Turns[t])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperWPF.Core
{
    internal class Field
    {
        public bool IsCovered { get; set; }
        public bool HasMine { get; set; }
        public bool HasFlag { get; set; }
        public int MinesAround { get; set; }

        public Field()
        {
            IsCovered = true;
            HasMine = false;
            HasFlag = false;
            MinesAround = 0;
        }

        public void ToggleFlag()
        {
            HasFlag = !HasFlag;
        }

        public void Uncover()
        {
            if (IsCovered)
            {
                IsCovered = false;

                if (HasFlag)
                {
                    HasFlag = false;
                    // Decrease flag count if it was flagged
                }
            }
        }

        public void SetMine()
        {
            HasMine = true;
        }

        public void SetMinesAround(int count)
        {
            MinesAround = count;
        }

        // Additional method to reset the field to its initial state
        public void Reset()
        {
            IsCovered = true;
            HasMine = false;
            HasFlag = false;
            MinesAround = 0;
        }
    }
}


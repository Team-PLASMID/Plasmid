using System;
using System.Collections.Generic;
using System.Text;

namespace Plasmid.Microbes
{
    public class DnaCycler
    {
        public DnaSequence Sequence { get; private set; }
        public int Index { get; private set; }

        public DnaCycler(DnaSequence dna)
        {
            this.Sequence = dna;
            this.Index = 0;
        }

        public Dna Next()
        {
            Dna dna = Sequence[Index];
            Index++;
            Index = Index % Sequence.Count;
            return dna;
        }
        public void SetCursor(int index)
        {
            Index = index % Sequence.Count;
        }
        public void Reset()
        {
            Index = 0;
        }
    }
}

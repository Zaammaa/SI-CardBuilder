using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects
{
    //An effect with a simple amount
    internal abstract class AmountEffect : Effect
    {

        public int amount = 1;
        public abstract int max {  get; }
        public abstract int min { get; }


    }
}

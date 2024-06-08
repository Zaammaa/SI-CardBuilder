using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.RangeOptions
{
    internal class S1Range : RangeOption
    {
        public override string Name => "Sacred 1 Range";
        public override double BaseProbability => 0.18;

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.card.Target.SpiritTarget)
                return false;
            return true;
        }

        public override Range OnSelected(Context context)
        {
            return new Range(true, 1, null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.RangeOptions
{
    internal class _3Range : RangeOption
    {
        public override string Name => "3 Range";
        public override double BaseProbability => 0.01;

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.card.Target.SpiritTarget)
                return false;
            return true;
        }

        public override Range OnSelected(Context context)
        {
            return new Range(false, 3, null);
        }
    }
}

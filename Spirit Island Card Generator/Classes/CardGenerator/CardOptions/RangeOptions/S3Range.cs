using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.RangeOptions
{
    internal class S3Range : RangeOption
    {
        public override double BaseProbability => 0.005;

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.card.Target.SpiritTarget)
                return false;
            if (context.settings.CustomEffectLevel < 3)
                return false;

            return true;
        }

        public override Range OnSelected(Context context)
        {
            return new Range(true, 3, null);
        }
    }
}

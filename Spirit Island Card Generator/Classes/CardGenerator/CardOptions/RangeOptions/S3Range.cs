using Spirit_Island_Card_Generator.Classes.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.RangeOptions
{
    [CustomEffect(2)]
    internal class S3Range : RangeOption
    {
        public override string Name => "Sacred 3 Range";
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

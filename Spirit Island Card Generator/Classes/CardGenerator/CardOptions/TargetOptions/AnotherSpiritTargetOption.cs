using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.TargetOptions
{
    internal class AnotherSpiritTargetOption : TargetOption
    {
        public override double BaseProbability => 0.03;

        public override int Complexity => 2;

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        public override Target OnChosen(Context context)
        {
            Target target = new Target();
            target.targetType = Target.TargetType.AnotherSpirit;
            return target;
        }
    }
}

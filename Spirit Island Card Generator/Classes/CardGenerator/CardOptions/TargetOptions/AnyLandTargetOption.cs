using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.TargetOptions
{
    internal class AnyLandTargetOption : TargetOption
    {
        public override string Name => "Any Land Target";
        public override double BaseProbability => 0.47;

        public override int Complexity => 0;

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        public override Target OnChosen(Context context)
        {
            Target target = new Target();
            target.targetType = Target.TargetType.Land;
            return target;
        }
    }
}

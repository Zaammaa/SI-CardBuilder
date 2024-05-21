using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.TargetOptions
{
    internal class NoBlightTargetOption : TargetOption
    {
        public override double BaseProbability => 0.04;

        public override int Complexity => 1;

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        public override Target OnChosen(Context context)
        {
            Target target = new Target();
            target.targetType = Target.TargetType.Land;
            target.landConditions.Add(TargetConditions.LandConditon.LandConditions.Noblight);
            return target;
        }
    }
}

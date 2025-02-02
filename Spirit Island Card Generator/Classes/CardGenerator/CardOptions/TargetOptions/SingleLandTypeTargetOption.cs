using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.TargetOptions
{
    internal class SingleLandTypeTargetOption : TargetOption
    {
        public override string Name => "Single Land Type Target: " + land;
        public override double BaseProbability => 0.01;

        public override int Complexity => 2;

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        LandConditions land;

        public override Target OnChosen(Context context)
        {
            Target target = new Target();
            target.targetType = Target.TargetType.Land;
            List<LandConditions> options = new List<LandConditions>() { LandConditions.Mountain, LandConditions.Jungle, LandConditions.Sands, LandConditions.Wetlands };
            LandConditions condition = Utils.ChooseRandomListElement(options, context.rng);
            land = condition;

            target.landConditions.Add(condition);
            return target;
        }
    }
}

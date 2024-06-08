using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator.CardOptions.TargetOptions
{
    internal class TwoLandTypesTargetOption : TargetOption
    {
        public override string Name => "Two Land Types Target: " + lands;
        public override double BaseProbability => 0.1;

        public override int Complexity => 1;

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        LandConditions lands;

        public override Target OnChosen(Context context)
        {
            Target target = new Target();
            target.targetType = Target.TargetType.Land;
            List<LandConditions> options = new List<LandConditions>() { LandConditions.MountainOrJungle, LandConditions.MountainOrSands, LandConditions.MountainOrWetlands, LandConditions.JungleOrSands, LandConditions.JungleOrWetlands, LandConditions.SandsOrWetlands };
            LandConditions condition = Utils.ChooseRandomListElement(options, context.rng);

            lands = condition;
            target.landConditions.Add(condition);
            return target;
        }
    }
}

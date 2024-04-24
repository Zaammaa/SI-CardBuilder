using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions
{
    [CostCondition]
    [CustomEffect(3)]
    internal class DiscardACardCondition : Condition
    {
        public override double BaseProbability => 0.05;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 2;

        public override double DifficultyMultiplier
        {
            get
            {
                return 0.7;
            }
        }

        public override string ConditionText => "You may discard a power card. If you do";

        public override bool ChooseEasierCondition(Context context)
        {
            return false;
        }

        public override bool ChooseHarderCondition(Context context)
        {
            return false;
        }

        public override IPowerLevel Duplicate()
        {
            DiscardACardCondition condition = new DiscardACardCondition();
            return condition;
        }

        public override void Initialize(Context context)
        {

        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }
    }
}

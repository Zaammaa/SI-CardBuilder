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
    [CustomEffect(4)]
    internal class ForgetACardCondition : Condition
    {
        public override double BaseProbability => 0.05;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 3;

        public override double DifficultyMultiplier
        {
            get
            {
                return 0.2;
            }
        }

        public override string ConditionText => "You may forget a power card. If you do";

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
            ForgetACardCondition condition = new ForgetACardCondition();
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

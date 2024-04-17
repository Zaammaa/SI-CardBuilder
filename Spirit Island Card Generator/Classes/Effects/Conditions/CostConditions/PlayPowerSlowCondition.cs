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
    internal class PlayPowerSlowCondition : Condition
    {
        public override double BaseProbability => 0.05;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 2;

        public override double DifficultyMultiplier
        {
            get
            {
                return 0.9;
            }
        }

        public override string ConditionText => "You may make this power {slow}. If you do";

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
            PlayPowerSlowCondition condition = new PlayPowerSlowCondition();
            return condition;
        }

        public override void Initialize(Context context)
        {

        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (!context.card.Fast)
                return false;

            return true;
        }
    }
}

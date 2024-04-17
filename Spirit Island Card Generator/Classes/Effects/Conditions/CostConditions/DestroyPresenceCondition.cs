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
    [SpiritCondition]
    internal class DestroyPresenceCondition : Condition
    {
        public override double BaseProbability => 0.1;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 2;

        public override double DifficultyMultiplier
        {
            get
            {
                return multipliers[presenceCost];
            }
        }

        public Dictionary<int, double> multipliers = new Dictionary<int, double>()
        {
            {1, 0.45},
            {2, 0.20},
            {3, 0.1}
        };

        public override string ConditionText => $"You may destroy {presenceCost} of your " + "{presence}.\n" + "If you do:";

        protected int presenceCost = 1;

        public override bool ChooseEasierCondition(Context context)
        {
            if (presenceCost > 1)
            {
                presenceCost -= 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool ChooseHarderCondition(Context context)
        {
            if (presenceCost < multipliers.Count)
            {
                presenceCost += 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override IPowerLevel Duplicate()
        {
            DestroyPresenceCondition condition = new DestroyPresenceCondition();
            condition.presenceCost = presenceCost;
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
